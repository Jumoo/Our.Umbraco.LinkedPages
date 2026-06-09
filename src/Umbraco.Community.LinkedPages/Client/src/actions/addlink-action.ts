import { UmbEntityActionBase } from "@umbraco-cms/backoffice/entity-action";
import { ADD_LINK_MODAL_TOKEN } from "../modals/types";
import { umbOpenModal } from "@umbraco-cms/backoffice/modal";

export class AddLinkAction extends UmbEntityActionBase<never> {
  override async execute() {
    this._openModal();
  }

  private async _openModal() {
    if (!this.args.unique) return;
    const returnedValue = await umbOpenModal(this, ADD_LINK_MODAL_TOKEN, {
      data: { uniqueId: this.args.unique },
    }).catch(() => undefined);
    console.log(returnedValue);
  }
}

export default AddLinkAction;
