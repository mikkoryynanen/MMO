using MMO.Database.Shared.Models;
using Riok.Mapperly.Abstractions;

namespace MMO.Auth.Models;

[Mapper]
public partial class Mapper
{
    public partial PlayerAccountDTO PlayerAccountToPlayerAccountDTO(PlayerAccount playerAccount);
    public partial SessionTokenDTO SessionTokenToSessionTokenDTO(SessionToken sessionToken);
}