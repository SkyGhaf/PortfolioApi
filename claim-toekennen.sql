DECLARE @Email NVARCHAR(256) = 'editor@test.nl';

INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
SELECT u.Id, 'MagVerwijderen', 'ja'
FROM AspNetUsers u
WHERE u.Email = @Email;