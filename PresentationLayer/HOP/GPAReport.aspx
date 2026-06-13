<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="GPAReport.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.GPAReport" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Academic Transcript</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />

    <style>
        .transcript {
            background: white;
            color: #111827;
            border: 1px solid #d1d5db;
            padding: 35px;
            border-radius: 8px;
        }

        .transcript-title {
            text-align: center;
            border-bottom: 2px solid #111827;
            padding-bottom: 15px;
            margin-bottom: 25px;
        }

        .info-table td {
            padding: 6px 12px;
            font-size: 14px;
        }

        .semester-title {
            background: #e5e7eb;
            padding: 10px;
            font-weight: bold;
            margin-top: 25px;
            border: 1px solid #d1d5db;
        }

        .signature-box {
            margin-top: 45px;
            display: flex;
            justify-content: space-between;
        }

        @media print {
            .no-print, .topbar {
                display: none !important;
            }

            body {
                background: white !important;
            }

            .page-shell {
                width: 100% !important;
                max-width: 100% !important;
                margin: 0 !important;
                padding: 0 !important;
            }

            .transcript {
                border: none;
                padding: 20px;
                border-radius: 0;
            }
        }
    </style>

    <script>
        function PrintTranscript() {
            window.print();
        }
    </script>
</head>

<body>
<form id="form1" runat="server">

    <nav class="navbar topbar px-4 py-3 no-print">
        <span class="navbar-brand fw-bold text-dark">
            <span class="blue-dot"></span>SIMS College Portal
        </span>

        <a href="AdminDashboard.aspx" class="btn btn-ios-light btn-sm">
            Back to Dashboard
        </a>
    </nav>

    <div class="container-fluid page-shell mt-4 mb-5 px-4">

        <div class="glass-card hero-card mb-4 no-print">
            <h2 class="fw-bold mb-1">Official Academic Transcript</h2>
            <p class="text-muted mb-0">
                Generate printable student transcript from database records.
            </p>
        </div>

        <div class="glass-card form-card mb-4 no-print">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Transcript Generator</h5>
                <div class="mini-help">
                    Select a student and generate official academic transcript.
                </div>
            </div>

            <div class="form-body">
                <div class="row g-4 align-items-end">

                    <div class="col-md-6">
                        <label class="form-label">Select Student</label>
                        <asp:DropDownList ID="ddlStudent" runat="server" CssClass="form-select" />
                    </div>

                    <div class="col-md-6">
                        <asp:Button ID="btnGenerate" runat="server"
                            Text="Generate Transcript"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnGenerate_Click" />

                        <asp:Button ID="btnPrint" runat="server"
                            Text="Print Transcript"
                            CssClass="btn btn-ios-light ms-2"
                            OnClientClick="PrintTranscript(); return false;" />
                    </div>

                    <div class="col-12">
                        <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold" />
                    </div>

                </div>
            </div>
        </div>

        <div class="transcript">

            <div class="transcript-title">
                <h3 class="fw-bold mb-1">STUDENT INFORMATION MANAGEMENT SYSTEM</h3>
                <h5 class="mb-0">OFFICIAL ACADEMIC TRANSCRIPT</h5>
            </div>

            <table class="info-table w-100 mb-4">
                <tr>
                    <td><strong>Student Name</strong></td>
                    <td>: <asp:Label ID="lblStudentName" runat="server" Text="-" /></td>
                    <td><strong>Matric Number</strong></td>
                    <td>: <asp:Label ID="lblMatric" runat="server" Text="-" /></td>
                </tr>
                <tr>
                    <td><strong>Programme</strong></td>
                    <td>: <asp:Label ID="lblProgramme" runat="server" Text="-" /></td>
                    <td><strong>Department</strong></td>
                    <td>: <asp:Label ID="lblDepartment" runat="server" Text="-" /></td>
                </tr>
                <tr>
                    <td><strong>Status</strong></td>
                    <td>: <asp:Label ID="lblStatus" runat="server" Text="-" /></td>
                    <td><strong>Academic Standing</strong></td>
                    <td>: <asp:Label ID="lblStanding" runat="server" Text="-" /></td>
                </tr>
                <tr>
                    <td><strong>CGPA</strong></td>
                    <td>: <asp:Label ID="lblCGPA" runat="server" Text="0.00" /></td>
                    <td><strong>Generated Date</strong></td>
                    <td>: <asp:Label ID="lblGeneratedDate" runat="server" Text="-" /></td>
                </tr>
            </table>

            <asp:PlaceHolder ID="phTranscript" runat="server"></asp:PlaceHolder>

            <div class="mt-4">
                <table class="table table-bordered">
                    <tr>
                        <th>Total Credits Earned</th>
                        <td><asp:Label ID="lblCreditsEarned" runat="server" Text="0" /></td>
                        <th>Cumulative CGPA</th>
                        <td><asp:Label ID="lblFinalCGPA" runat="server" Text="0.00" /></td>
                    </tr>
                </table>
            </div>

            <div class="signature-box">
                <div>
                    <p>Prepared By:</p>
                    <br />
                    <p>______________________________</p>
                    <p>Head of Programme</p>
                </div>

                <div>
                    <p>Verified By:</p>
                    <br />
                    <p>______________________________</p>
                    <p>Academic Office</p>
                </div>
            </div>

        </div>

    </div>

</form>
</body>
</html>