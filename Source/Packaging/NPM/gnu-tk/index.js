// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

"use strict";

const platform = require("./platform");
const path = require("path");
const fs = require("fs");
const childProcess = require("child_process");

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

  const processHandle = childProcess.spawn(processPath, args, {
    stdio: "inherit",
  });

  process.on("SIGINT", () => {}); // let the launched process handle it
  processHandle.on("exit", (code, signal) => exit(rid, code));
}

function getProcessPath(rid) {
  let processPath = path.resolve(
    path.join(__dirname, "platforms", rid, "gnu-tk"),
  );
  if (rid.startsWith("win-")) processPath += ".exe";
  return processPath;
}

function exit(rid, code) {
  if (!rid.startsWith("win-") && process.stdout.isTTY) {
    // Reset terminal cursor key and keypad modes.
    process.stdout.write("\x1b[?1l\x1b>");
  }

  process.exit(code);
}
