using System;
using System.Drawing;
using System.Windows.Forms;

namespace Stock_Managemnet
{
    public sealed class CorrectionGuideForm : Form
    {
        public CorrectionGuideForm()
        {
            Text = "Fix Production / Sale Mistakes";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(640, 420);
            UiStyles.Apply(this);

            var body = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.None,
                BackColor = SystemColors.Window,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Text =
@"If a mistake was made during raw material entry or production, use this order:

0) VOID STOCK IN (Transaction History tab) — wrong raw material qty/supplier
   - Select the stock-in row and click Void Stock In.
   - Quantity is removed from inventory.
   - Supplier dues exclude the voided purchase.
   - Then stock in again with the correct quantity.

1) VOID THE SALE (Invoices tab)
   - Select the invoice and click Void Sale.
   - Stock returns to inventory.
   - Accounts Receivable and Sales Revenue are reversed.
   - Any payment on that invoice is reversed too.

2) REVERSE PRODUCTION (Production History tab)
   - Select the production run and click Reverse Production.
   - Finished goods are removed from stock.
   - Raw materials used in that run are returned.
   - Product unit cost is restored to the value before that run.

3) FIX THE ROOT CAUSE
   - Raw material price: Inventory tab, edit the raw material.
   - Wrong stock-in qty: Transaction History → Void Stock In, then stock in again.
   - Wrong/missing materials: Production tab, edit the production recipe.

4) RE-RUN PRODUCTION
   - Run production again with the corrected recipe/materials.

5) SELL AGAIN
   - Stock out / create a new invoice from Inventory.

Notes
- Reverse production only works when the produced quantity is still in stock.
- Void the sale first if the finished goods were already sold.
- Void stock-in only works when the stocked quantity is still available (reverse production first if used).
- Voided invoices, stock-ins, and reversed production runs stay in history for audit.
- Sales Profit, customer due, and accounts ignore voided sales.
- Supplier dues ignore voided stock-ins.

Undo a wrong void/reverse
- Transaction History: select a Voided stock-in -> Restore Stock In.
- Invoices tab: select a Voided invoice -> Restore Sale.
- Production History tab: select a Reversed run -> Restore Production.
- Restore only works if enough stock/materials are available."
            };

            var close = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Size = new Size(90, 30),
                Location = new Point(530, 375)
            };
            close.Click += (s, e) => Close();

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16, 16, 16, 52) };
            panel.Controls.Add(body);

            Controls.Add(panel);
            Controls.Add(close);
            AcceptButton = close;
        }
    }
}
