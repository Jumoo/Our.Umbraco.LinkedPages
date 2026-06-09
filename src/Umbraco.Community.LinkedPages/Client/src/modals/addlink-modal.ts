import {
  css,
  customElement,
  html,
  state,
} from "@umbraco-cms/backoffice/external/lit";
import {
  UmbModalBaseElement,
  UmbModalExtensionElement,
} from "@umbraco-cms/backoffice/modal";
import { AddLinkModalData, AddLinkModalValue } from "./types.js";
import {
  UmbTreeElement,
  UmbTreeSelectionConfiguration,
} from "@umbraco-cms/backoffice/tree";
import { UmbSelectionChangeEvent } from "@umbraco-cms/backoffice/event";

@customElement("addlink-dialog")
export class AddLinkDialogElement
  extends UmbModalBaseElement<AddLinkModalData, AddLinkModalValue>
  implements UmbModalExtensionElement<AddLinkModalData, AddLinkModalValue>
{
  @state()
  selection?: string;

  private _handleCancel() {
    this.modalContext?.submit();
  }

  private _handleSubmit() {
    this.value = { selected: this.selection };
    this.modalContext?.submit();
  }

  #onSelectionChange(e: UmbSelectionChangeEvent) {
    e.stopPropagation();
    const element = e.target as UmbTreeElement;
    const value = element.getSelection();
    this.selection = value[0];

    if (!this.selection) {
    }
  }

  render() {
    return html`
      <umb-body-layout>
        <div slot="header">
          <h1>Add Link</h1>
        </div>
        <uui-box>${this.renderPicker()}</uui-box>
        <div slot="actions">
          <div class="buttons">
            <uui-button
              .label=${this.localize.term("general_close")}
              look="default"
              color="default"
              @click=${this._handleCancel}
            ></uui-button>
            <uui-button
              .label=${this.localize.term("general_add")}
              look="primary"
              color="positive"
              @click=${this._handleSubmit}
              .disabled=${!this.selection ||
              this.selection == this.data?.uniqueId}
            ></uui-button>
          </div>
        </div>
      </umb-body-layout>
    `;
  }

  renderPicker() {
    const alias = "Umb.Tree.Document";

    const selectionConfig: UmbTreeSelectionConfiguration = {
      multiple: false,
      selectable: true,
      selection: [this.selection ?? null],
    };

    return html`<uui-box headline="placeholder select"
      ><umb-tree
        .alias=${alias}
        .props=${{
          hideTreeItemActions: true,
          hideTreeRoot: true,
          selectionConfiguration: selectionConfig,
        }}
        @selection-change=${this.#onSelectionChange}
      ></umb-tree
    ></uui-box>`;
  }

  static styles = css`
    .buttons {
      display: flex;
      gap: 6px;
    }
  `;
}

export const element = AddLinkDialogElement;
