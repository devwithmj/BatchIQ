import Providers from "./providers";
import "./globals.css";

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body data-new-gr-c-s-check-loaded="14.1110.0" data-gr-ext-installed="">
        {" "}
        <Providers>{children}</Providers>
      </body>
    </html>
  );
}
