DECLARE @Email NVARCHAR(256) = 'editor@test.nl';
DECLARE @Rol NVARCHAR(256) = 'Editor';

INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u, AspNetRoles r
WHERE u.Email = @Email AND r.Name = @Rol;