export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Umbraco Community Linked Pages Entrypoint",
    alias: "Umbraco.Community.LinkedPages.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint.js"),
  },
];
