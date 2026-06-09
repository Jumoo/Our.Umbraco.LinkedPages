import { UmbModalToken } from "@umbraco-cms/backoffice/modal";

export type LinkedPagesModalData = {
  uniqueId: string;
};

export type LinkedPagesModalValue = {};

export type AddLinkModalData = {
  uniqueId: string;
};

export type AddLinkModalValue = {
  selected?: string;
};

export const LINKED_PAGES_MODAL_TOKEN = new UmbModalToken<
  LinkedPagesModalData,
  LinkedPagesModalValue
>("LinkedPages.Modal", {
  modal: {
    type: "sidebar",
    size: "small",
  },
});

export const ADD_LINK_MODAL_TOKEN = new UmbModalToken<
  AddLinkModalData,
  AddLinkModalValue
>("AddLink.Modal", {
  modal: {
    type: "sidebar",
    size: "small",
  },
});
