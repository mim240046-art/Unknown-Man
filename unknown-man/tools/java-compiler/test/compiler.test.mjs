import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs";
import os from "node:os";
import path from "node:path";
import { collectJavaFiles, compileJava } from "../src/compiler.mjs";

test("collectJavaFiles is recursive and platform-neutral", () => {
  const root = fs.mkdtempSync(path.join(os.tmpdir(), "unknown-man-java-"));
  fs.mkdirSync(path.join(root, "nested"));
  fs.writeFileSync(path.join(root, "One.java"), "final class One {}\n");
  fs.writeFileSync(path.join(root, "nested", "Two.java"), "final class Two {}\n");
  fs.writeFileSync(path.join(root, "ignored.txt"), "ignored\n");
  assert.deepEqual(
    collectJavaFiles(root).map(file => path.basename(file)),
    ["One.java", "Two.java"]
  );
});

test("compileJava fails clearly when javac is unavailable", () => {
  const root = fs.mkdtempSync(path.join(os.tmpdir(), "unknown-man-java-"));
  fs.writeFileSync(path.join(root, "One.java"), "final class One {}\n");
  assert.throws(
    () => compileJava({ sourceDirectory: root, outputDirectory: path.join(root, "out"), javac: "definitely-missing-javac" }),
    /Unable to run definitely-missing-javac/
  );
});
