// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

import url from "node:url";
import path from "node:path";

export function getMetaDirName() {
  return import.meta.dirname ?? getFallback();

  // 'import.meta.dirname' was added to Node v21.2.0 and back-ported to v20.11.0 (LTS).
  // In older Node releases it's simply undefined.
  // This function is a fallback implementation.
  function getFallback() {
    const fileName = url.fileURLToPath(import.meta.url);
    return path.dirname(fileName);
  }
}
