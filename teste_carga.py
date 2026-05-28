import asyncio
import httpx
import random
import time
import sys

URL = "http://localhost:50010/api/Product/InsertAndPublish"

TOTAL_REQUESTS = 50000
CONCURRENCY = 300  # Baixamos um pouco para dar fôlego ao Windows local
LOG_INTERVAL = 5000

progress_counter = 0

if sys.platform == 'win32':
    asyncio.set_event_loop_policy(asyncio.WindowsSelectorEventLoopPolicy())

def generate_payload(i: int):
    return {
        "name": f"product-{i}-{random.randint(1,999999)}",
        "description": "load test product",
        "category": "test",
        "unitPrice": random.randint(1, 1000),
        "stockQuantity": random.randint(1, 100),
        "unitOfMeasure": "unit",
        "manufacturer": "load-test",
        "createdAt": "2026-05-28T21:28:48.567Z"
    }

async def send_request(client: httpx.AsyncClient, i: int, sem: asyncio.Semaphore):
    global progress_counter
    async with sem:
        payload = generate_payload(i)
        try:
            # Colocamos um timeout menor (5s). Se travar local, ele cancela logo e avisa
            r = await client.post(URL, json=payload, timeout=5.0)
            
            progress_counter += 1
            if progress_counter % LOG_INTERVAL == 0 or progress_counter == TOTAL_REQUESTS:
                print(f"[Progresso] {progress_counter}/{TOTAL_REQUESTS} enviadas...")
            
            return r.status_code
            
        except httpx.ConnectError:
            progress_counter += 1
            print(f"🛑 [Windows Travou] Sockets locais esgotados ou API caiu na Req #{i}")
            return "connect_error"
        except Exception as e:
            progress_counter += 1
            print(f"💥 [Erro] Req #{i} falhou por: {type(e).__name__}")
            return "error"

async def main():
    sem = asyncio.Semaphore(CONCURRENCY)

    print(f"🚀 Iniciando teste: {TOTAL_REQUESTS} requisições.")
    
    # IMPORTANTE: Forçamos o HTTPX a fechar as conexões TCP de forma agressiva assim que terminar
    # Isso impede que o Python trave as portas locais do Windows em TIME_WAIT
    limits = httpx.Limits(max_keepalive_connections=10, max_connections=CONCURRENCY)
    
    async with httpx.AsyncClient(limits=limits, headers={"Connection": "close"}) as client:
        start = time.time()
        tasks = [send_request(client, i, sem) for i in range(TOTAL_REQUESTS)]
        results = await asyncio.gather(*tasks)
        end = time.time()

    success = sum(1 for r in results if r in [200, 201])
    errors = len(results) - success

    print(f"\n==== FINALIZADO ====\nSucesso: {success} | Erros: {errors}\nTempo: {end - start:.2f}s")

if __name__ == "__main__":
    asyncio.run(main())