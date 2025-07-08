import Providers from "./providers";
import "./globals.css";
import { Toaster } from "@/components/ui/sonner";

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body data-new-gr-c-s-check-loaded="14.1111.0" data-gr-ext-installed="">
        {" "}
        <Providers>{children}</Providers>
        <Toaster />
      </body>
    </html>
  );
}
