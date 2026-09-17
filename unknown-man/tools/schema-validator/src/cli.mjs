#!/usr/bin/env node
import {validateContentDir} from "./validator.mjs";
const dir=process.argv[2]??"shared/content";
const issues=validateContentDir(dir);
if(issues.length){for(const x of issues) console.error(`${x.file}: ${x.message}`); process.exit(1);}
console.log(`Validated UNKNOWN MAN content: ${dir}`);
