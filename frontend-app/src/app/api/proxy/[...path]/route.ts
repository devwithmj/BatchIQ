import { NextRequest, NextResponse } from 'next/server';

// Get backend URL from environment or default to localhost:5000
const BACKEND_URL = process.env.BACKEND_URL || 'http://localhost:5000';

interface RouteParams {
  params: Promise<{ path: string[] }>;
}

export async function GET(
  request: NextRequest,
  context: RouteParams
) {
  const params = await context.params;
  return proxyRequest(request, params.path, 'GET');
}

export async function POST(
  request: NextRequest,
  context: RouteParams
) {
  const params = await context.params;
  return proxyRequest(request, params.path, 'POST');
}

export async function PUT(
  request: NextRequest,
  context: RouteParams
) {
  const params = await context.params;
  return proxyRequest(request, params.path, 'PUT');
}

export async function DELETE(
  request: NextRequest,
  context: RouteParams
) {
  const params = await context.params;
  return proxyRequest(request, params.path, 'DELETE');
}

async function proxyRequest(request: NextRequest, path: string[], method: string) {
  try {
    const url = `${BACKEND_URL}/api/${path.join('/')}`;
    const searchParams = request.nextUrl.searchParams;
    const fullUrl = searchParams.toString() ? `${url}?${searchParams}` : url;

    // Get request body for POST/PUT requests
    let body;
    if (method === 'POST' || method === 'PUT') {
      body = await request.text();
    }

    // Forward headers (excluding host and some Next.js specific headers)
    const headers: Record<string, string> = {};
    request.headers.forEach((value, key) => {
      const lowerKey = key.toLowerCase();
      if (!['host', 'x-forwarded-for', 'x-forwarded-proto', 'x-forwarded-host'].includes(lowerKey)) {
        headers[key] = value;
      }
    });

    console.log(`Proxying ${method} request to: ${fullUrl}`);

    const response = await fetch(fullUrl, {
      method,
      headers,
      body,
    });

    const responseData = await response.text();
    
    // Forward response headers
    const responseHeaders: Record<string, string> = {};
    response.headers.forEach((value, key) => {
      responseHeaders[key] = value;
    });

    return new NextResponse(responseData, {
      status: response.status,
      headers: responseHeaders,
    });
  } catch (error) {
    console.error('Proxy error:', error);
    return NextResponse.json(
      { error: 'Proxy request failed', details: error instanceof Error ? error.message : 'Unknown error' },
      { status: 500 }
    );
  }
}
