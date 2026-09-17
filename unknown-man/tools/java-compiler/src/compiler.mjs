import fs from "node:fs";
import path from "node:path";
import { spawnSync } from "node:child_process";

export function collectJavaFiles(rootDirectory) {
  const files = [];
  const visit = directory => {
    for (const entry of fs.readdirSync(directory, { withFileTypes: true })) {
      const absolutePath = path.join(directory, entry.name);
      if (entry.isDirectory()) visit(absolutePath);
      else if (entry.isFile() && entry.name.endsWith(".java")) files.push(absolutePath);
    }
  };
  visit(rootDirectory);
  return files.sort();
}

export function compileJava({ sourceDirectory, outputDirectory, javac = "javac" }) {
  const sourceFiles = collectJavaFiles(sourceDirectory);
  if (sourceFiles.length === 0) throw new Error(`No Java sources found in ${sourceDirectory}`);
  fs.rmSync(outputDirectory, { recursive: true, force: true });
  fs.mkdirSync(outputDirectory, { recursive: true });
  const result = spawnSync(javac, ["-d", outputDirectory, ...sourceFiles], {
    encoding: "utf8",
    shell: false
  });
  if (result.error) throw new Error(`Unable to run ${javac}: ${result.error.message}`);
  if (result.status !== 0) {
    const detail = [result.stdout, result.stderr].filter(Boolean).join("\n").trim();
    throw new Error(`Java compilation failed with exit code ${result.status}${detail ? `:\n${detail}` : ""}`);
  }
  return { sourceCount: sourceFiles.length, outputDirectory };
}
