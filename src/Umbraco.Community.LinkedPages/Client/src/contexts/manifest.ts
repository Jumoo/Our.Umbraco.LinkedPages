import { ManifestGlobalContext } from "@umbraco-cms/backoffice/extension-registry";

const contexts: Array<ManifestGlobalContext> = [
  {
    type: "globalContext",
    alias: "linkedPages.context",
    name: "linked pages context",
    js: () => import(`./context.js`),
  },
];

export const manifests = [...contexts];
