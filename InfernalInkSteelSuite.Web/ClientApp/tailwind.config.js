/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        app: '#050510',
        surface: '#101020',
        'surface-alt': '#1A1A30',
        primary: '#8A2BE2',
        'primary-hover': '#B266FF',
        accent: '#00E5FF',
        danger: '#FF4081',
        'text-primary': '#FFFFFF',
        'text-secondary': '#B0B0D0',
        'text-muted': '#7070A0',
      },
    },
  },
  plugins: [],
}
