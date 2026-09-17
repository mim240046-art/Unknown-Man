package com.unknownman.core;
import java.util.List;
public interface GameModule { String id(); List<String> dependencies(); void initialize(); void start(); void stop(); List<String> healthCheck(); }
