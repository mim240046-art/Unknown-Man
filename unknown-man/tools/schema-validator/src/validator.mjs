import fs from "node:fs";
import path from "node:path";

const ID=/^unknownman:(evidence|anomaly|puzzle)\/[a-z0-9_/-]+$/;
export function readJson(file){return JSON.parse(fs.readFileSync(file,"utf8"));}
function issue(file,message){return {file,message};}
export function validateContentDir(dir){
 const out=[];
 const eFile=path.join(dir,"evidence.json"), aFile=path.join(dir,"anomalies.json"), pFile=path.join(dir,"puzzles.json");
 for(const f of [eFile,aFile,pFile]) if(!fs.existsSync(f)) out.push(issue(f,"required content file is missing"));
 if(out.length) return out;
 const evidence=readJson(eFile), anomalies=readJson(aFile), puzzles=readJson(pFile);
 for(const [file,doc,kind] of [[eFile,evidence,"evidence"],[aFile,anomalies,"anomaly"],[pFile,puzzles,"puzzle"]]){
  if(doc.schemaVersion!==1) out.push(issue(file,"schemaVersion must equal 1"));
  if(!Array.isArray(doc.entries)) {out.push(issue(file,"entries must be an array")); continue;}
  const ids=new Set();
  for(const x of doc.entries){
   if(typeof x.id!=="string"||!ID.test(x.id)||!x.id.startsWith(`unknownman:${kind}/`)) out.push(issue(file,`invalid ${kind} id: ${x.id}`));
   if(ids.has(x.id)) out.push(issue(file,`duplicate id: ${x.id}`)); ids.add(x.id);
  }
 }
 const evidenceIds=new Set(evidence.entries.map(x=>x.id));
 for(const e of evidence.entries){
  if(!Number.isInteger(e.dayAvailable)||e.dayAvailable<1||e.dayAvailable>7) out.push(issue(eFile,`${e.id}: dayAvailable must be 1..7`));
  for(const r of e.relatedEvidence??[]) if(!evidenceIds.has(r)) out.push(issue(eFile,`${e.id}: missing related evidence ${r}`));
 }
 for(const a of anomalies.entries){
  if(a.minDay>a.maxDay) out.push(issue(aFile,`${a.id}: minDay exceeds maxDay`));
  if(a.minDistance>=a.maxDistance) out.push(issue(aFile,`${a.id}: minDistance must be below maxDistance`));
  if(a.probability<0||a.probability>1) out.push(issue(aFile,`${a.id}: probability must be 0..1`));
  if(a.canAffectCriticalContent!==false) out.push(issue(aFile,`${a.id}: anomalies may not affect critical content`));
 }
 for(const p of puzzles.entries){
  if(!Array.isArray(p.clues)||p.clues.length===0) out.push(issue(pFile,`${p.id}: at least one clue is required`));
  for(const req of p.requirements??[]) if(req.startsWith("unknownman:evidence/")&&!evidenceIds.has(req)) out.push(issue(pFile,`${p.id}: missing required evidence ${req}`));
 }
 return out;
}
