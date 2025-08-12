#!/usr/bin/env node

// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

import platform from "./platform.js";
import path from "node:path";
import fs from "node:fs";
import { spawn } from "node:child_process";
import url from "node:url";

try {
  run(process.argv.slice(2));
} catch (error) {
  console.error("Error: GNU-TK: " + error.message);
  process.exit(1);
}

function run(args) {
  const rid = platform.NET.getRid();

  const processPath = getProcessPath(rid);
  if (!fs.existsSync(processPath)) {
    throw new Error(`Platform '${rid}' is not supported.`);
  }

  const processHandle = spawn(processPath, args, {
    stdio: "inherit",
  });

  process.on("SIGINT", () => {}); // let the launched process handle it
  processHandle.on("exit", (code, signal) => exit(rid, code));
}

function getProcessPath(rid) {
  let processPath = path.resolve(
    path.join(
      import.meta.dirname ?? getMetaDirNameFallback(),
      "platforms",
      rid,
      "gnu-tk",
    ),
  );
  if (rid.startsWith("win-")) processPath += ".exe";
  return processPath;
}

// 'import.meta.dirname' was added to Node v21.2.0 and back-ported to v20.11.0 (LTS).
// In older Node releases it's simply undefined.
// This function is a fallback implementation.
function getMetaDirNameFallback() {
  const fileName = url.fileURLToPath(import.meta.url);
  return path.dirname(fileName);
}

function exit(rid, code) {
  if (!rid.startsWith("win-") && process.stdout.isTTY) {
    // Reset terminal cursor key and keypad modes.
    process.stdout.write("\x1b[?1l\x1b>");
  }

  process.exit(code);
}
