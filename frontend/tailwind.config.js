/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: "#f0f9ff",
          100: "#e0f2fe",
          200: "#bae6fd",
          300: "#7dd3fc",
          400: "#38bdf8",
          500: "#0ea5e9",
          600: "#0284c7",
          700: "#0369a1",
          800: "#075985",
          900: "#0c3d66",
        },
        medical: {
          blue: "#1e40af",
          green: "#059669",
          red: "#dc2626",
          yellow: "#d97706",
        },
      },
      spacing: {
        "128": "32rem",
        "144": "36rem",
      },
      borderRadius: {
        lg: "0.5rem",
        xl: "0.75rem",
      },
      fontSize: {
        xs: ["0.75rem", { lineHeight: "1rem" }],
        sm: ["0.875rem", { lineHeight: "1.25rem" }],
        base: ["1rem", { lineHeight: "1.5rem" }],
        lg: ["1.125rem", { lineHeight: "1.75rem" }],
        xl: ["1.25rem", { lineHeight: "1.75rem" }],
      },
    },
  },
  plugins: [],
  important: true,
  darkMode: "class",
};
