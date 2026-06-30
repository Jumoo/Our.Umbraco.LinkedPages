import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import {
  deleteRemoveLink,
  getChildLinks,
  getIgnoredTypeAlias,
  getParentLinks,
  LinkedPageInfo,
  postCreateLink,
} from "../api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { UmbArrayState } from "@umbraco-cms/backoffice/observable-api";
import { tryExecute } from "@umbraco-cms/backoffice/resources";

export class LinkedPagesContext extends UmbControllerBase {
  #relationCount = 0;
  readonly relationCount = this.#relationCount;

  #parents = new UmbArrayState<LinkedPageInfo>([], (x) => x.relationId);
  readonly parents = this.#parents.asObservable();

  #children = new UmbArrayState<LinkedPageInfo>([], (x) => x.relationId);
  readonly children = this.#children.asObservable();

  #host: UmbControllerHost;
  constructor(host: UmbControllerHost) {
    super(host);
    this.#host = host;
    this.provideContext(LINKED_PAGES_CONTEXT_TOKEN, this);
  }

  async getRelationCount(uniqueId: string) {
    let children = await tryExecute(
      this.#host,
      getChildLinks({
        query: { key: uniqueId },
      }),
    );
    const childCount = children.data?.length ?? 0;
    let parents = await tryExecute(
      this.#host,
      getParentLinks({
        query: { key: uniqueId },
      }),
    );
    const parentCount = parents.data?.length ?? 0;
    const relationCount = parentCount + childCount;
    return relationCount;
  }

  async getParents(uniqueId: string) {
    let parents = await tryExecute(
      this.#host,
      getParentLinks({
        query: {
          key: uniqueId,
        },
      }),
    );
    if (parents.data != null) this.#parents.setValue(parents.data);
  }

  async getChildren(uniqueId: string) {
    let children = await tryExecute(
      this.#host,
      getChildLinks({
        query: { key: uniqueId },
      }),
    );
    if (children.data != null) this.#children.setValue(children.data);
  }

  async getIgnored() {
    let ignoredTypes = await tryExecute(this.#host, getIgnoredTypeAlias());
    return ignoredTypes.data;
  }

  async removeLink(id: number, currentPage: string) {
    let children = await tryExecute(
      this.#host,
      deleteRemoveLink({
        query: { key: id, currentPage: currentPage },
      }),
    );
    if (children.data != null) this.#children.setValue(children.data);
  }

  async addLink(parentId: string, childId: string) {
    let children = await tryExecute(
      this.#host,
      postCreateLink({
        query: { parent: parentId, child: childId },
      }),
    );
    if (children.data != null) this.#children.setValue(children.data);
  }
}

export default LinkedPagesContext;
export const LINKED_PAGES_CONTEXT_TOKEN =
  new UmbContextToken<LinkedPagesContext>(LinkedPagesContext.name);
