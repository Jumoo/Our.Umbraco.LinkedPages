import { UmbEntityActionBase } from "@umbraco-cms/backoffice/entity-action";
import { umbOpenModal } from "@umbraco-cms/backoffice/modal";
import { LINKED_PAGES_MODAL_TOKEN } from "../modals/types";

export class LinkedPagesAction extends UmbEntityActionBase<never> {
  override async execute() {
    this._openModal();
  }

  private async _openModal() {
    if (!this.args.unique) return;
    const returnedValue = await umbOpenModal(this, LINKED_PAGES_MODAL_TOKEN, {
      data: {
        uniqueId: this.args.unique,
      },
    }).catch(() => undefined);
    console.debug(returnedValue);
  }
}

export default LinkedPagesAction;
