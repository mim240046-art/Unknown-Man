export const UNKNOWN_MAN_STATES = ["DORMANT","OBSERVING","STALKING","HIDING","INVESTIGATING","REPOSITIONING","APPROACHING","CHASE","ATTACK","RETREAT","SEARCHING","CONFUSED","SCRIPTED_EVENT","FINAL_HUNT","FINAL_BOSS"] as const;
export type UnknownManState = typeof UNKNOWN_MAN_STATES[number];
export type Platform = "bedrock";
export interface GameEvent<T=unknown>{readonly id:string; readonly schemaVersion:1; readonly gameTime:number; readonly sourceModule:string; readonly correlationId:string; readonly payload:T;}
export interface SavePayload{currentDay:1|2|3|4|5|6|7; storyFlags:Record<string,boolean>; evidenceIds:string[]; puzzleStates:Record<string,string>; unlockedLocationIds:string[]; monsterForm:1|2|3|4|5|6|7; monsterState:UnknownManState; endingRequirements:Record<string,boolean>;}
export interface SaveEnvelope{schemaVersion:1; buildVersion:string; platform:Platform; worldIdentity:string; timestamp:string; payload:SavePayload;}
export interface GameModule{readonly id:string; readonly dependencies:readonly string[]; initialize():void; start():void; stop():void; healthCheck():readonly string[];}
export interface SchedulerPort{schedule(delayTicks:number,task:()=>void):string; cancel(taskId:string):boolean;}
export interface PersistencePort{load(worldIdentity:string):SaveEnvelope|null; save(snapshot:SaveEnvelope):void; checkpoint(snapshot:SaveEnvelope,reason:string):void;}
