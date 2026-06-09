import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { LinkedPageInfo } from "../api";
import {
  css,
  customElement,
  html,
  property,
  when,
} from "@umbraco-cms/backoffice/external/lit";

@customElement("linked-item")
export class LinkedItemView extends UmbLitElement {
  @property({ type: Object })
  item?: LinkedPageInfo;

  @property({ type: Boolean })
  showType: boolean = true;

  @property({ type: Boolean })
  showRemove: boolean = true;

  #onRemove() {
    this.dispatchEvent(
      new CustomEvent("remove-link", {
        detail: {
          item: this.item,
        },
        bubbles: true,
        composed: true,
      }),
    );
  }

  render() {
    return html`<!--${JSON.stringify(this.item, null, 2)}-->
      <uui-box>
        <div class="linked-page-info">
          <div>
            <div><strong>${this.item?.name}</strong></div>
            <em>${this.item?.path}</em>
            <div>
              ${when(
                this.showType,
                () => html`<em>${this.item?.relationType}</em>`,
              )}
            </div>
          </div>

          <div>
            ${when(
              this.showRemove,
              () =>
                html`<uui-button
                  label="Remove"
                  color="danger"
                  look="primary"
                  @click=${this.#onRemove}
                ></uui-button>`,
            )}
          </div>
        </div>
      </uui-box>`;
  }

  static styles = css`
    .linked-page-info {
      display: flex;
      justify-content: space-between;
    }
  `;
}

export default LinkedItemView;

declare global {
  interface HTMLElementTagNameMap {
    "linked-item": LinkedItemView;
  }
}
