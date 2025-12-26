'use client';

import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { styled } from '@mui/material/styles';
import SecurityIcon from '@mui/icons-material/Security';
import CodeIcon from '@mui/icons-material/Code';
import GitHubIcon from '@mui/icons-material/GitHub';
import ArticleIcon from '@mui/icons-material/Article';

const StyledBox = styled('div')(({ theme }) => ({
  alignSelf: 'center',
  width: '100%',
  height: 400,
  marginTop: theme.spacing(8),
  borderRadius: (theme.vars || theme).shape.borderRadius,
  outline: '1px solid',
  outlineColor: 'hsla(220, 25%, 80%, 0.2)',
  border: '1px solid',
  borderColor: (theme.vars || theme).palette.grey[200],
  boxShadow: '0 0 12px 8px hsla(220, 25%, 80%, 0.2)',
  background: `linear-gradient(135deg, ${theme.palette.primary.dark} 0%, ${theme.palette.primary.main} 100%)`,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  position: 'relative',
  overflow: 'hidden',
  '&::before': {
    content: '""',
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    background: 'radial-gradient(circle at 30% 50%, rgba(255,255,255,0.1) 0%, transparent 50%)',
  },
  [theme.breakpoints.up('sm')]: {
    marginTop: theme.spacing(10),
    height: 500,
  },
  ...theme.applyStyles('dark', {
    boxShadow: '0 0 24px 12px hsla(210, 100%, 25%, 0.2)',
    backgroundImage: `url(${process.env.TEMPLATE_IMAGE_URL || 'https://mui.com'}/static/screenshots/material-ui/getting-started/templates/dashboard-dark.jpg)`,
    outlineColor: 'hsla(220, 20%, 42%, 0.1)',
    borderColor: (theme.vars || theme).palette.grey[700],
  }),
}));

const CodeBlock = styled(Box)(({ theme }) => ({
  fontFamily: 'monospace',
  fontSize: '0.875rem',
  padding: theme.spacing(3),
  borderRadius: theme.shape.borderRadius,
  backgroundColor: 'rgba(0, 0, 0, 0.6)',
  color: '#00ff00',
  textAlign: 'left',
  maxWidth: '600px',
  margin: '0 auto',
  position: 'relative',
  zIndex: 1,
  '& .comment': {
    color: '#888',
  },
  '& .keyword': {
    color: '#569cd6',
  },
  '& .string': {
    color: '#ce9178',
  },
}));

export default function Hero() {
  return (
    <Box
      id="hero"
      sx={(theme) => ({
        width: '100%',
        backgroundRepeat: 'no-repeat',
        backgroundImage:
          'radial-gradient(ellipse 80% 50% at 50% -20%, hsl(210, 100%, 90%), transparent)',
        ...theme.applyStyles('dark', {
          backgroundImage:
            'radial-gradient(ellipse 80% 50% at 50% -20%, hsl(210, 100%, 16%), transparent)',
        }),
      })}
    >
      <Container
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          pt: { xs: 14, sm: 20 },
          pb: { xs: 8, sm: 12 },
        }}
      >
        <Stack
          spacing={2}
          useFlexGap
          sx={{ alignItems: 'center', width: { xs: '100%', sm: '70%' } }}
        >
          {/* Security Badge */}
          <Box
            sx={{
              display: 'flex',
              alignItems: 'center',
              gap: 1,
              px: 2,
              py: 1,
              borderRadius: 2,
              backgroundColor: 'primary.main',
              color: 'white',
            }}
          >
            <SecurityIcon fontSize="small" />
            <Typography variant="caption" fontWeight="bold">
              Enterprise-Grade Authentication
            </Typography>
          </Box>

          {/* Main Headline */}
          <Typography
            variant="h1"
            sx={{
              display: 'flex',
              flexDirection: { xs: 'column', sm: 'row' },
              alignItems: 'center',
              fontSize: 'clamp(2.5rem, 8vw, 3.5rem)',
              textAlign: 'center',
              fontWeight: 700,
            }}
          >
            Secure, Scalable&nbsp;
            <Typography
              component="span"
              variant="h1"
              sx={(theme) => ({
                fontSize: 'inherit',
                fontWeight: 'inherit',
                color: 'primary.main',
                ...theme.applyStyles('dark', {
                  color: 'primary.light',
                }),
              })}
            >
              Authentication Service
            </Typography>
          </Typography>

          {/* Subheading */}
          <Typography
            variant="h5"
            sx={{
              textAlign: 'center',
              color: 'text.secondary',
              width: { sm: '100%', md: '85%' },
              fontWeight: 400,
              lineHeight: 1.6,
            }}
          >
            Built with Domain-Driven Design and Clean Architecture. 
            API-first identity management for modern engineering teams.
          </Typography>
          <Stack
            direction={{ xs: 'column', sm: 'row' }}
            spacing={2}
            useFlexGap
            sx={{ pt: 3 }}
          >
            <Button
              variant="contained"
              color="primary"
              size="large"
              startIcon={<CodeIcon />}
              sx={{
                minWidth: '180px',
                fontWeight: 600,
                textTransform: 'none',
                fontSize: '1rem',
              }}
            >
              View API Docs
            </Button>
            <Button
              variant="outlined"
              color="primary"
              size="large"
              startIcon={<ArticleIcon />}
              sx={{
                minWidth: '180px',
                fontWeight: 600,
                textTransform: 'none',
                fontSize: '1rem',
              }}
            >
              Architecture Guide
            </Button>
            <Button
              variant="text"
              color="primary"
              size="large"
              startIcon={<GitHubIcon />}
              sx={{
                minWidth: '140px',
                fontWeight: 600,
                textTransform: 'none',
                fontSize: '1rem',
              }}
            >
              GitHub
            </Button>
          </Stack>

          {/* Technical Emphasis */}
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ textAlign: 'center', pt: 2, maxWidth: '600px' }}
          >
            Designed for developers who value clean code, security best practices, 
            and maintainable architecture. No bloat, just robust identity management.
          </Typography>
        </Stack>
        <StyledBox id="image" />
      </Container>
    </Box>
  );
}
