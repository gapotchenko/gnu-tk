// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

"use strict";

const os = require("os");

// Gets a .NET runtime identifier.
function getNETRid() {
  let platform = os.platform();
  let arch = os.arch();

  switch (platform) {
    case "linux":
      switch (arch) {
        case "x64":
          return "linux-x64";
        case "arm64":
          return "linux-arm64";
      }
    case "darwin":
      switch (arch) {
        case "x64":
          return "osx-x64";
        case "arm64":
          return "osx-arm64";
      }
    case "win32":
      switch (arch) {
        case "x32":
        case "ia32":
          return "win-x86";
        case "x64":
          return "win-x64";
        case "arm64":
          return "win-arm64";
      }
  }

  throw new Error(
    `Platform '${platform}.${arch}' is not a supported Node.js environment.`,
  );
}

module.exports = { NET: { getRid: getNETRid } };
