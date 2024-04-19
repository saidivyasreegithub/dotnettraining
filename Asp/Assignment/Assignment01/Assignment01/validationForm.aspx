<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ValidatorForm.aspx.cs" Inherits="Validation_Assignment.ValidatorForm" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
       body {
    font-family: 'Times New Roman';
    background-color: #f0f0f0;
    margin: 0;
    padding: 0;
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 100vh;
}

#form1 {
    background-color: palevioletred;
    width: 40%;
    max-width: 648px;
    margin: 50px auto;
    padding: 20px;
    border-radius: 20px;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
}

h2 {
    text-align: center;
    text-decoration: underline;
}

label {
    display: block;
    font-weight: bold;
    margin-bottom: 5px;
}

input[type="text"]
{
    width: 100%;
    padding: 10px;
    margin-bottom: 15px;
    border: 1px solid #ccc;
    border-radius: 5px;
    box-sizing: border-box;
}

input[type="submit"] {
    width: 30%;
    padding: 12px;
    background-color: #007bfe;
    color: #eee;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    font-size: 16px;
}

input[type="submit"]:hover {
    background-color: #0056b3;
}

.error-message {
    color: red;
    font-style: italic;
}


button:hover {
    background-color: #ffcc00;
}

.validation-summary {
    color: red;
    text-align: center;
    margin-top: 20px;
}

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Registration Form</h2>

        <div>
            <label for="txtName">Name:</label>
            <asp:TextBox ID="txtName" runat="server" Width="200px" BackColor="#FFFFCC" BorderStyle="None" Height="30px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredNameValidator" runat="server" ControlToValidate="txtName" ErrorMessage="Name is Required" ForeColor="Red">*</asp:RequiredFieldValidator>
        
            <label for="txtFamilyName">Family Name:</label>
            <asp:TextBox ID="txtFamilyName" runat="server" Width="200px" BackColor="#FFFFCC" ForeColor="Black" BorderStyle="None" Height="30px"></asp:TextBox>
            <asp:RequiredFieldValidator CssClass="error-message" ID="RequiredFamilyNameValidator" runat="server" ControlToValidate="txtFamilyName" ErrorMessage="Family Name is Required" ForeColor="Red">*</asp:RequiredFieldValidator>
            <asp:CompareValidator CssClass="error-message" ID="CompareNameValidator" runat="server" ControlToValidate="txtName" ControlToCompare="txtFamilyName" Operator="NotEqual" ErrorMessage="Family Name must be different from Name" ForeColor="Red"></asp:CompareValidator>
       
            <label for="txtAddress">Address:</label>
            <asp:TextBox ID="txtAddress" runat="server" Width="200px" BackColor="#FFFFCC" BorderStyle="None" Height="30px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredAddressValidator" runat="server" ControlToValidate="txtAddress" ErrorMessage="Address is Required" ForeColor="Red">*</asp:RequiredFieldValidator>
            <asp:CustomValidator ID="MinLengthAddressValidator" runat="server" ControlToValidate="txtAddress" ErrorMessage="Address must be at least 2 characters long" ValidateValue="ValidateMinLength" ForeColor="Red"></asp:CustomValidator>
            <asp:CustomValidator ID="CustomValidator2" runat="server" ControlToValidate="txtAddress" ErrorMessage="Address must be at least 2 letters long" ForeColor="Red" ClientValidationFunction="validateAddressLength"></asp:CustomValidator>

      
            <label for="txtCity">City:</label>
            <asp:TextBox ID="txtCity" runat="server" Width="200px" BackColor="#FFFFCC" BorderStyle="None" Height="30px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredCityValidator" runat="server" ControlToValidate="txtCity" ErrorMessage="City is Required" ForeColor="Red">*</asp:RequiredFieldValidator>
            <asp:CustomValidator ID="MinLengthCityValidator" runat="server" ControlToValidate="txtCity" ErrorMessage="City must be at least 2 characters long" ValidateValue="ValidateMinLength" ForeColor="Red"></asp:CustomValidator>
            <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="txtCity" ErrorMessage="City must be at least 2 letters long" ForeColor="Red" ClientValidationFunction="validateCityLength"></asp:CustomValidator>


            <label for="txtZipCode">Zip Code:</label>
            <asp:TextBox ID="txtZipCode" runat="server" Width="200px" BackColor="#FFFFCC" BorderStyle="None" Height="30px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtZipCode" ErrorMessage="Zip Code is Required" ForeColor="Red">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="regexZipCode" runat="server" ControlToValidate="txtZipCode" ErrorMessage="Zip Code must be 5 digits" ValidationExpression="\d{5}" ForeColor="Red"></asp:RegularExpressionValidator>
        
            <label for="txtPhone">Phone:</label>
            <asp:TextBox ID="txtPhone" runat="server" Width="200px" BackColor="#FFFFCC" BorderStyle="None" Height="30px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPhone" ErrorMessage="Phone Number is Required" ForeColor="Red">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="regexPhone" runat="server" ControlToValidate="txtPhone" ErrorMessage="format (XX-XXXXXXX or XXX-XXXXXXX)" ValidationExpression="\d{10}" ForeColor="Red"></asp:RegularExpressionValidator>
       
            <label for="txtEmail">E-Mail:</label>
            <asp:TextBox ID="txtEmail" runat="server" Width="200px" BackColor="#FFFFCC" BorderStyle="None" Height="30px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is Required" ForeColor="Red">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEmail" ErrorMessage="Invalid Email Format (xyz@xyz.xyz)" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+" ForeColor="Red"></asp:RegularExpressionValidator>
       
            <asp:Button ID="Button1" runat="server" Text="Check" OnClick="Button1_Click" />
      
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ForeColor="Red" ShowMessageBox="True" />
        </div>
    </form>
</body>

    <script>
        function validateCityLength(source, args) {
            var cityInput = document.getElementById('<%= txtCity.ClientID %>').value;
        if (cityInput.length >= 2) {
            args.IsValid = true;
        } else {
            args.IsValid = false;
        }
        }

        function validateAddressLength(source, args) {
            var addressInput = document.getElementById('<%= txtAddress.ClientID %>').value;
            if (addressInput.length >= 2) {
                args.IsValid = true;
            } else {
                args.IsValid = false;
            }
        }
    </script>

</html>
