<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="ResetPassword.aspx.cs"
    Inherits="Student_Information_Management_System.Lecturer.ResetPassword" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Lecturer Reset Password</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet" />
    <link href="../../Content/LoginTheme.css" rel="stylesheet" />

    <style>
        .password-wrapper {
            position: relative;
        }

        .password-wrapper .form-control {
            padding-right: 48px;
        }

        .password-toggle {
            position: absolute;
            top: 50%;
            right: 14px;
            transform: translateY(-50%);
            border: none;
            background: transparent;
            color: #64748b;
            font-size: 18px;
            cursor: pointer;
            padding: 0;
            line-height: 1;
        }

        .password-toggle:hover {
            color: #2563eb;
        }
    </style>

    <script>
        function togglePassword(inputId, iconId) {
            var input = document.getElementById(inputId);
            var icon = document.getElementById(iconId);

            if (input.type === "password") {
                input.type = "text";
                icon.className = "bi bi-eye-slash";
            } else {
                input.type = "password";
                icon.className = "bi bi-eye";
            }
        }
    </script>
</head>

<body>
<form id="form1" runat="server">

    <div class="login-card">

        <h3 class="login-title">RESET PASSWORD</h3>

        <div class="login-subtitle">
            Create a new secure lecturer password
        </div>

        <div class="mb-3">
            <label class="form-label fw-semibold">New Password</label>

            <div class="password-wrapper">
                <asp:TextBox ID="txtPassword" runat="server"
                    CssClass="form-control"
                    TextMode="Password"
                    placeholder="Enter new password" />

                <button type="button"
                    class="password-toggle"
                    onclick="togglePassword('<%= txtPassword.ClientID %>', 'iconPassword')">
                    <i id="iconPassword" class="bi bi-eye"></i>
                </button>
            </div>
        </div>

        <div class="mb-3">
            <label class="form-label fw-semibold">Confirm Password</label>

            <div class="password-wrapper">
                <asp:TextBox ID="txtConfirmPassword" runat="server"
                    CssClass="form-control"
                    TextMode="Password"
                    placeholder="Confirm new password" />

                <button type="button"
                    class="password-toggle"
                    onclick="togglePassword('<%= txtConfirmPassword.ClientID %>', 'iconConfirmPassword')">
                    <i id="iconConfirmPassword" class="bi bi-eye"></i>
                </button>
            </div>
        </div>

        <asp:Button ID="btnReset" runat="server"
            Text="Reset Password"
            CssClass="btn-login"
            OnClick="btnReset_Click" />

        <asp:Button ID="btnLoginPage" runat="server"
            Text="Back to Login"
            CssClass="btn btn-outline-primary w-100 mt-3"
            OnClick="btnLoginPage_Click" />

        <asp:Label ID="lblMessage" runat="server"
            CssClass="d-block text-center mt-3 fw-bold" />

    </div>

</form>
</body>
</html>