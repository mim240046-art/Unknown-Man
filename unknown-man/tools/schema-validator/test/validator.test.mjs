import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs";
import os from "node:os";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { validateContentDir } from "../src/validator.mjs";

const source = fileURLToPath(new URL("../../../shared/content/", import.meta.url));
const messages = issues => issues.map(issue => issue.message).join(" | ");

function copyFixture() {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), "unknown-man-"));
  for (const name of ["evidence.json", "anomalies.json", "puzzles.json"]) {
    const fixture = fileURLToPath(new URL(`../../../shared/content/${name}`, import.meta.url));
    fs.copyFileSync(fixture, path.join(directory, name));
  }
  return directory;
}

test("production seed content is valid", () => {
  assert.deepEqual(validateContentDir(source), []);
});

test("broken evidence relationship is rejected", () => {
  const directory = copyFixture();
  const file = path.join(directory, "evidence.json");
  const document = JSON.parse(fs.readFileSync(file, "utf8"));
  document.entries[0].relatedEvidence = ["unknownman:evidence/missing"];
  fs.writeFileSync(file, JSON.stringify(document));
  assert.match(messages(validateContentDir(directory)), /missing related evidence/);
});

test("unsafe anomaly is rejected", () => {
  const directory = copyFixture();
  const file = path.join(directory, "anomalies.json");
  const document = JSON.parse(fs.readFileSync(file, "utf8"));
  document.entries[0].canAffectCriticalContent = true;
  fs.writeFileSync(file, JSON.stringify(document));
  assert.match(messages(validateContentDir(directory)), /may not affect critical content/);
});
