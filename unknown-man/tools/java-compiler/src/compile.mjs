#!/usr/bin/env node
import path from "node:path";
import { fileURLToPath } from "node:url";
import { compileJava } from "./compiler.mjs";

const projectRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "../../..");
try {
  const result = compileJava({
    sourceDirectory: path.join(projectRoot, "java", "src", "main", "java"),
    outputDirectory: path.join(projectRoot, "builds", "java-contracts")
  });
  console.log(`Compiled ${result.sourceCount} Java contract files into ${result.outputDirectory}`);
} catch (error) {
  console.error(error instanceof Error ? error.message : String(error));
  process.exit(1);
}
