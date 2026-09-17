package com.unknownman.core;
import java.util.Objects;
public record GameEvent<T>(String id,int schemaVersion,long gameTime,String sourceModule,String correlationId,T payload){
 public GameEvent { Objects.requireNonNull(id); Objects.requireNonNull(sourceModule); Objects.requireNonNull(correlationId); if(schemaVersion!=1) throw new IllegalArgumentException("Unsupported event schema"); if(gameTime<0) throw new IllegalArgumentException("gameTime"); }
}
