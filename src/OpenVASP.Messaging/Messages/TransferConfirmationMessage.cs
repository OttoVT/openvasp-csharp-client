﻿using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.Messaging.Messages
{
    public class TransferConfirmationMessage : MessageBase
    {
        public TransferConfirmationMessage(
            Message message,
            Originator originator,
            Beneficiary beneficiary,
            TransferReply transfer,
            Transaction transaction,
            VaspInformation vasp)
        {
            MessageType = MessageType.TransferDispatch;
            Message = message;
            Originator = originator;
            Beneficiary = beneficiary;
            Transfer = transfer;
            Transaction = transaction;
            VASP = vasp;
        }

        public Originator Originator { get; private set; }

        public Beneficiary Beneficiary { get; private set; }

        public TransferReply Transfer { get; private set; }

        public Transaction Transaction { get; private set; }

        public Message Message { get; private set; }

        public VaspInformation VASP { get; private set; }

        public static string GetMessageCode(TransferConfirmationMessageCode messageCode)
        {
            return messageCode.ToString();
        }
        public enum TransferConfirmationMessageCode
        {
            TransferConfirmed = 1,
            TransferNotConfirmedDispatchNotValid = 2,
            TransferNotConfirmedAssetsNotReceived = 3,
            TransferNotConfirmedWrongAmount = 4,
            TransferNotConfirmedWrongAsset = 5,
            TransferNotConfirmedTransactionDataMissmatch = 6,
        }
    }
}