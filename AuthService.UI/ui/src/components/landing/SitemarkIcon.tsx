'use client';
import SvgIcon from '@mui/material/SvgIcon';

export default function SitemarkIcon() {
  return (
    <SvgIcon 
      sx={{ 
        height: 24, 
        width: 180, 
        mr: 2,
        '& path': {
          fill: 'url(#authGradient)',
        },
      }} 
      color="inherit"
    >
      <svg
        width={180}
        height={24}
        viewBox="0 0 180 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <defs>
          <linearGradient id="authGradient" x1="0%" y1="0%" x2="100%" y2="100%">
            <stop offset="0%" stopColor="#667EEA" />
            <stop offset="100%" stopColor="#764BA2" />
          </linearGradient>
          <linearGradient id="coreGradient" x1="0%" y1="0%" x2="100%" y2="100%">
            <stop offset="0%" stopColor="#00C6FF" />
            <stop offset="100%" stopColor="#0072FF" />
          </linearGradient>
        </defs>
        <path
          d="M18 8h-1V6c0-2.76-2.24-5-5-5S7 3.24 7 6v2H6c-1.1 0-2 .9-2 2v10c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V10c0-1.1-.9-2-2-2ZM9 6c0-1.66 1.34-3 3-3s3 1.34 3 3v2H9V6ZM6 10h12v10H6V10ZM12 17c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2Z"
        />
        <text
          x="28"
          y="18"
          fill="url(#authGradient)"
          fontFamily="Roboto, Helvetica, Arial, sans-serif"
          fontWeight="700"
          fontSize="16"
        >
          AuthService
        </text>
        <text
          x="122"
          y="18"
          fill="url(#coreGradient)"
          fontFamily="Roboto, Helvetica, Arial, sans-serif"
          fontWeight="700"
          fontSize="16"
        >
          Core
        </text>
      </svg>
    </SvgIcon>
  );
}
