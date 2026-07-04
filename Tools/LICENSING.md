# Offline licensing

This app uses **offline machine-bound activation**. Each customer laptop needs a unique activation key from you.

## One-time developer setup

1. Build the solution in Visual Studio (Release).
2. Run `Tools\LicenseKeyGenerator\bin\Release\License Key Generator.exe` once.
   - It creates developer keys automatically on first launch.
   - It writes the public key to `Services\Licensing\LicensePublicKey.xml`.
3. **Rebuild the main ELECTRONICS app** so the new public key is embedded in the client `.exe`.
4. Keep `Tools\LicenseKeyGenerator\keys\private.xml` **only on your PC**. Never copy it to a customer laptop.

> If you click **Create Developer Keys** again, all old customer keys stop working until you rebuild the main app and re-issue keys.

## Selling to a new customer

1. Install the app on the customer's laptop (SQL Express + Release folder).
2. Run the app. The **Activation** screen shows a **Machine ID** (example: `A1B2-C3D4-E5F6-G7H8`).
3. Customer sends you that Machine ID (phone / WhatsApp / visit).
4. Open **License Key Generator** on your PC.
5. Enter **customer name** + **Machine ID** → **Generate Key** → **Copy**.
6. Send the activation key to the customer (SMS, paper, USB, etc.).
7. Customer pastes the key → **Activate** → app runs normally.

License is stored at:

`C:\ProgramData\SS Electronics\license.key`

## New laptop for same customer

1. Install app on the new laptop.
2. New laptop = new Machine ID.
3. Generate a **new** activation key for the same customer name + new Machine ID.
4. Old laptop key does not work on the new machine (by design).

## What you ship to customers

- `Stock Managemnet.exe` + DLLs + `Assets\` + `App.config`
- **Do NOT ship** `License Key Generator.exe` or `private.xml`

## Customer install checklist

1. Windows 10/11
2. .NET Framework 4.7.2+
3. SQL Server Express (`.\SQLEXPRESS`)
4. Copy Release folder
5. Activate with your key
6. Login and change password
