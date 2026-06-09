import { UMB_DOCUMENT_ENTITY_TYPE } from "@umbraco-cms/backoffice/document";

const manifest: UmbExtensionManifest[] = [
  {
    type: "entityAction",
    kind: "default",
    alias: "Jumoo.LinkedPagesAction",
    name: "Jumoo Linked Pages Action",
    weight: 10,
    api: () => import("./linkedpages-action.js"),
    forEntityTypes: [UMB_DOCUMENT_ENTITY_TYPE],
    meta: {
      icon: "icon-add",
      label: "Linked Pages",
    },
  },
  // {
  //   type: "entityAction",
  //   kind: "default",
  //   alias: "Jumoo.AddLinkAction",
  //   name: "Jumoo Add Link Action",
  //   weight: 10,
  //   api: () => import("./addlink-action.js"),
  //   forEntityTypes: [UMB_DOCUMENT_ENTITY_TYPE],
  //   meta: {
  //     icon: "icon-add",
  //     label: "Add Link",
  //   },
  // },
];

export const manifests = [...manifest];
