// Copyright 2023 Yubico AB
//
// Licensed under the Apache License, Version 2.0 (the "License").
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Yubico.Core.Iso7816;
using Yubico.YubiKey.Fido2.PinProtocols;
using Yubico.YubiKey.Fido2.Cbor;

namespace Yubico.YubiKey.Fido2.Commands
{
    /// <summary>
    /// Delete a credential.
    /// </summary>
    /// <remarks>
    /// The partner Response class is <see cref="Fido2Response"/>. This command
    /// does not return any data, it only returns "success" or "failure", and has
    /// some FIDO2-specific error information.
    /// <para>
    /// This deletes a FIDO2 credential from the YubiKey. It is possible there is
    /// some large blob data associated with that credential. This command will
    /// not delete that data.
    /// </para>
    /// </remarks>
    public class DeleteCredentialCommand : IYubiKeyCommand<Fido2Response>
    {
        private const int SubCmdDeleteCredential = 0x06;
        private const int KeyCredentialId = 2;

        private readonly CredentialManagementCommand _command;

        /// <inheritdoc />
        public YubiKeyApplication Application => _command.Application;

        // The default constructor explicitly defined. We don't want it to be
        // used.
        private DeleteCredentialCommand()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructs a new instance of <see cref="DeleteCredentialCommand"/>.
        /// </summary>
        /// <param name="credentialId">
        /// The <c>CredentialId</c> of the credential to delete.
        /// </param>
        /// <param name="pinUvAuthToken">
        /// The PIN/UV Auth Token built from the PIN. This is the encrypted token
        /// key.
        /// </param>
        /// <param name="authProtocol">
        /// The Auth Protocol used to build the Auth Token.
        /// </param>
        public DeleteCredentialCommand(
            CredentialId credentialId,
            ReadOnlyMemory<byte> pinUvAuthToken,
            PinUvAuthProtocolBase authProtocol)
        {
            _command = new CredentialManagementCommand(
                SubCmdDeleteCredential, EncodeParams(credentialId), pinUvAuthToken, authProtocol);
        }

        /// <inheritdoc />
        public CommandApdu CreateCommandApdu() => _command.CreateCommandApdu();

        /// <inheritdoc />
        public Fido2Response CreateResponseForApdu(ResponseApdu responseApdu) =>
            new Fido2Response(responseApdu);

        // This method encodes the parameters. For
        // DeleteCredentialCommand, the parameters consist of only the
        // credentialId, and it is encoded as
        //   map
        //     02 encoding of credentialID
        private static byte[] EncodeParams(CredentialId credentialId)
        {
            return new CborMapWriter<int>()
                .Entry(KeyCredentialId, credentialId)
                .Encode();
        }
    }
}
