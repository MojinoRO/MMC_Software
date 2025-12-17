namespace MMC_Software
{
    public static class RespositorioBCryp
    {
        // este es el valor que va manejar bcryp para el hashear las claves es mas lento pero seguro 
        private const int WorkFactor = 10;

        public static string HashPassword(string Contraseña)
        {
            return BCrypt.Net.BCrypt.HashPassword(Contraseña, workFactor: WorkFactor);
        }
        public static bool VerifyPassword(string Contraseña, string HashAlmacenado)
        {
            // BCrypt.Verify maneja la comparación segura y extrae el salt del storedHash.
            return BCrypt.Net.BCrypt.Verify(Contraseña, HashAlmacenado);
        }
    }
}
