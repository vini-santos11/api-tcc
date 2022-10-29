namespace Domain.Exceptions
{
    public static class Messages
    {
        public static string BlockedUser { get => "Usuário bloqueado."; }
        public static string PasswordChangeRequired { get => "Troca de senha obrigatória."; }
        public static string InvalidProfile { get => "Perfil inválido"; }
        public static string InvalidUserOrPassword { get => "Usuário ou senha inválido."; }
        public static string FieldRequired { get => "O campo {0} é obrigátorio."; }
        public static string AlreadyRegisteredUser { get => "Já existe um cadastro para este documento."; }
        public static string ErrorRegisteringUser { get => "Erro ao cadastrar usuário."; }
        public static string ContactNotFound { get => "Contato não encontrado."; }
        public static string UserNotFound { get => "Usuário não encontrado"; }
        public static string ProductNotFound { get => "Produto não encontrado."; }
        public static string FailureConvertDataToExcel { get => "Falha ao converter dados para excel."; }
        public static string ContactIsNotLegal { get => "Só é possível compra de fornecedores"; }
    }
}
