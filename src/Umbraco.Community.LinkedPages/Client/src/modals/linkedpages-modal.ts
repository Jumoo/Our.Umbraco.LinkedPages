import {
  css,
  customElement,
  html,
  property,
  state,
  when,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import {
  UmbModalExtensionElement,
  umbOpenModal,
} from "@umbraco-cms/backoffice/modal";
import type { UmbModalContext } from "@umbraco-cms/backoffice/modal";
import {
  ADD_LINK_MODAL_TOKEN,
  type LinkedPagesModalData,
  type LinkedPagesModalValue,
} from "./types.js";
import LinkedPagesContext, {
  LINKED_PAGES_CONTEXT_TOKEN,
} from "../contexts/context.js";
import { LinkedPageInfo } from "../api/types.gen.js";
import { UmbDocumentDetailRepository } from "@umbraco-cms/backoffice/document";

@customElement("linkedpages-dialog")
export class LinkedPagesDialogElement
  extends UmbLitElement
  implements
    UmbModalExtensionElement<LinkedPagesModalData, LinkedPagesModalValue>
{
  #linkedPagesContexts?: LinkedPagesContext;

  #documentRepository = new UmbDocumentDetailRepository(this);

  #documentUnique = "";

  @state()
  protected relationCount: number = 0;

  @state()
  protected parents?: LinkedPageInfo[];

  @state()
  protected childlinks?: LinkedPageInfo[];

  @state()
  protected ignoredTypes?: string[];

  @property({ attribute: false })
  modalContext?: UmbModalContext<LinkedPagesModalData, LinkedPagesModalValue>;

  @property({ attribute: false })
  data?: LinkedPagesModalData;

  @state()
  private _documentName = "";

  private _handleCancel() {
    this.modalContext?.submit();
  }

  constructor() {
    super();
    this.consumeContext(
      LINKED_PAGES_CONTEXT_TOKEN,
      async (_linkedPagesContext) => {
        if (!_linkedPagesContext) return;
        this.#linkedPagesContexts = _linkedPagesContext;

        this.observe(_linkedPagesContext.parents, (_parents) => {
          this.parents = _parents;
          this.relationCount =
            (this.parents?.length ?? 0) + (this.childlinks?.length ?? 0);
          //console.log("parents observed", _parents);
        });

        this.observe(_linkedPagesContext.children, (_children) => {
          this.childlinks = _children;
          this.relationCount =
            (this.parents?.length ?? 0) + (this.childlinks?.length ?? 0);
          //console.log("children observed", _children);
        });

        this.ignoredTypes = await _linkedPagesContext.getIgnored();
      },
    );
  }
  async connectedCallback(): Promise<void> {
    super.connectedCallback();
    this.#documentUnique = this.data?.uniqueId ?? "";
    const { data } = await this.#documentRepository.requestByUnique(
      this.#documentUnique,
    );
    if (!this.data || !data) return;

    this._documentName = data.variants[0].name;
    //console.log(this._documentName);

    this.#linkedPagesContexts?.getParents(this.data?.uniqueId);
    this.#linkedPagesContexts?.getChildren(this.data?.uniqueId);
  }

  #onRemove(e: CustomEvent) {
    //console.log("detail", e.detail);
    this.#linkedPagesContexts?.removeLink(
      e.detail.item.relationId,
      this.data?.uniqueId ?? "",
    );
  }

  render() {
    return html`
      <umb-body-layout>
        <div slot="header">
          <h1>Linked Pages</h1>
        </div>
        <div>
          <uui-box
            >${when(
              this.relationCount == 0,
              () => html`This content has no relations.`,
            )}${this.renderRelationCount()}${this.renderParents()}
            ${this.renderLinks()}${this.renderIgnored()}
            ${this.renderAdd()}</uui-box
          >
        </div>
        <div slot="actions">
          <uui-button
            id="cancel"
            .label=${this.localize.term("general_close")}
            @click=${this._handleCancel}
          ></uui-button>
        </div>
      </umb-body-layout>
    `;
  }

  renderRelationCount() {
    console.log("relation count:", this.relationCount);
    return html`${when(
        this.relationCount > 0,
        () =>
          html`The current page <strong>${this._documentName}</strong> is linked
            to the following
            <span
              >${when(
                this.relationCount > 1,
                () => html`${this.relationCount} pages`,
              )}${when(this.relationCount == 1, () => html`page`)}</span
            >`,
      )}
      <p class="abstract"></p>`;
  }

  renderParents() {
    var items = this.parents?.map((x) => {
      return html`<linked-item .item=${x} .showRemove=${false}></linked-item>`;
    });

    if (this.parents?.length)
      return html`<h5>Parent Links</h5>
        <p><em>You can only remove parent links from the parent node</em></p>

        <div class="links">${items}</div>`;
  }

  renderLinks() {
    var items = this.childlinks?.map((x) => {
      return html`<linked-item
        .item=${x}
        @remove-link=${this.#onRemove}
      ></linked-item>`;
    });

    if (this.childlinks?.length)
      return html`<h5>Linked Pages</h5>

        <div class="links">${items}</div>`;
  }

  renderAdd() {
    return html`<div class="add">
      <uui-button
        .label=${this.localize.term("general_add")}
        look="primary"
        color="positive"
        @click=${this.#openAdd}
      ></uui-button>
    </div>`;
  }

  async #openAdd() {
    if (!this.data?.uniqueId) return;
    //console.log(this.args);
    const returnedValue = await umbOpenModal(this, ADD_LINK_MODAL_TOKEN, {
      data: {
        uniqueId: this.data?.uniqueId,
      },
    }).catch(() => undefined);
    if (!returnedValue?.selected) return;
    this.#linkedPagesContexts?.addLink(
      this.data.uniqueId,
      returnedValue.selected,
    );
  }

  renderIgnored() {
    const ignoreCount = this.ignoredTypes?.length ?? 0;
    return html`${when(
      ignoreCount > 0,
      () =>
        html`Not showing relations of
        ${when(ignoreCount > 1, () => html`types`)}
        ${when(ignoreCount == 1, () => html`type`)}
        ${this.ignoredTypes?.join(", ")}`,
    )}`;
  }

  static styles = css`
    .links {
      display: flex;
      flex-direction: column;
      gap: var(--uui-size-4);
    }

    .add {
      display: flex;
      justify-content: flex-end;
      margin: 10px 0;
    }
  `;
}

export const element = LinkedPagesDialogElement;
