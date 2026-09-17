package com.unknownman.core;
import java.time.Instant; import java.util.Map; import java.util.Set;
public record SaveSnapshot(int schemaVersion,String buildVersion,String worldIdentity,Instant timestamp,Payload payload){
 public SaveSnapshot { if(schemaVersion!=1) throw new IllegalArgumentException("Unsupported save schema"); if(worldIdentity==null||worldIdentity.isBlank()) throw new IllegalArgumentException("worldIdentity"); if(payload==null) throw new IllegalArgumentException("payload"); }
 public record Payload(int currentDay,Set<String> storyFlags,Set<String> evidenceIds,Map<String,String> puzzleStates,Set<String> unlockedLocationIds,int monsterForm,UnknownManState monsterState,Map<String,Boolean> endingRequirements){
  public Payload { if(currentDay<1||currentDay>7) throw new IllegalArgumentException("currentDay"); if(monsterForm<1||monsterForm>7) throw new IllegalArgumentException("monsterForm"); storyFlags=Set.copyOf(storyFlags); evidenceIds=Set.copyOf(evidenceIds); puzzleStates=Map.copyOf(puzzleStates); unlockedLocationIds=Set.copyOf(unlockedLocationIds); endingRequirements=Map.copyOf(endingRequirements); }
 }
}
