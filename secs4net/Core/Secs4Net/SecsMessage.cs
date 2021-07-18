﻿using System;
using System.Buffers;

namespace Secs4Net
{
    public sealed record SecsMessage
    {
        public override string ToString() => $"'S{S}F{F}' {(ReplyExpected ? "W" : string.Empty)} {Name ?? string.Empty}";

        /// <summary>
        /// message stream number
        /// </summary>
        public byte S { get; }

        /// <summary>
        /// message function number
        /// </summary>
        public byte F { get; }

        /// <summary>
        /// expect reply message
        /// </summary>
        public bool ReplyExpected { get; internal set; }

        /// <summary>
        /// the root item of message
        /// </summary>
        public Item? SecsItem { get; init; }

        public string? Name { get; set; }
        public int Id { get; internal set; }
        public ushort DeviceId { get; internal set; }

        /// <summary>
        /// constructor of SecsMessage
        /// </summary>
        /// <param name="s">message stream number</param>
        /// <param name="f">message function number</param>
        /// <param name="replyExpected">expect reply message</param>
        public SecsMessage(byte s, byte f, bool replyExpected = true)
        {
            if (s > 0b0111_1111)
            {
                throw new ArgumentOutOfRangeException(nameof(s), s, Resources.SecsMessageStreamNumberMustLessThan127);
            }

            S = s;
            F = f;
            ReplyExpected = replyExpected;
        }

        internal void EncodeHeaderTo(IBufferWriter<byte> buffer) 
            => new MessageHeader(
                DeviceId,
                ReplyExpected,
                S,
                F,
                MessageType.DataMessage,
                Id
                ).EncodeTo(buffer);
    }
}
