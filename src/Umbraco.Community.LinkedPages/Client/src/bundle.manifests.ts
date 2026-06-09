import { manifests as entrypoints } from "./entrypoints/manifest.js";
import { manifests as actions } from "./actions/manifest.js";
import { manifests as modals } from "./modals/manifest.js";
import { manifests as contexts } from "./contexts/manifest.ts";

import "./components/index.js";

// Job of the bundle is to collate all the manifests from different parts of the extension and load other manifests
// We load this bundle from umbraco-package.json
export const manifests: Array<UmbExtensionManifest> = [
  ...entrypoints,
  ...actions,
  ...modals,
  ...contexts,
];
