const manifest: UmbExtensionManifest[] = [
  {
    type: "modal",
    alias: "LinkedPages.Modal",
    name: "Linked Pages Modal",
    js: () => import("./linkedpages-modal.js"),
  },
  {
    type: "modal",
    alias: "AddLink.Modal",
    name: "Add Link Modal",
    js: () => import("./addlink-modal.js"),
  },
];

export const manifests = [...manifest];
