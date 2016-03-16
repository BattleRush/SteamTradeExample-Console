using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample
{
	public enum ETradeOfferState
	{

		/// <summary>
		/// (-1) Error, Internal Error
		/// </summary>
		ETradeOfferError = -1,

		/// <summary>
		/// (0) Not found
		/// </summary>
		ETradeOfferNotFound = 0,

		/// <summary>
		/// (1) Invalid
		/// </summary>
		ETradeOfferStateInvalid = 1,

		/// <summary>
		/// (2) This trade offer has been sent, neither party has acted on it yet.
		/// </summary>
		ETradeOfferStateActive = 2,

		/// <summary>
		/// (3) The trade offer was accepted by the recipient and items were exchanged.
		/// </summary>
		ETradeOfferStateAccepted = 3,

		/// <summary>
		/// (4) The recipient made a counter offer
		/// </summary>
		ETradeOfferStateCountered = 4,

		/// <summary>
		/// (5) The trade offer was not accepted before the expiration date
		/// </summary>
		ETradeOfferStateExpired = 5,

		/// <summary>
		/// (6) The sender cancelled the offer
		/// </summary>
		ETradeOfferStateCanceled = 6,

		/// <summary>
		/// (7) The recipient declined the offer
		/// </summary>
		ETradeOfferStateDeclined = 7,

		/// <summary>
		/// (8) Some of the items in the offer are no longer available (indicated by the missing flag in the output)
		/// </summary>
		ETradeOfferStateInvalidItems = 8,

		/// <summary>
		/// (9) The offer hasn't been sent yet and is awaiting email/mobile confirmation. The offer is only visible to the sender.
		/// </summary>
		ETradeOfferStateCreatedNeedsConfirmation = 9,

		/// <summary>
		/// (10) Either party canceled the offer via email/mobile. The offer is visible to both parties, even if the sender canceled it before it was sent.
		/// </summary>
		ETradeOfferStateCanceledBySecondFactor = 10,

		/// <summary>
		/// (11) The trade has been placed on hold. The items involved in the trade have all been removed from both parties' inventories and will be automatically delivered in the future
		/// </summary>
		ETradeOfferStateInEscrow = 11,
	}
}
