from playwright.sync_api import sync_playwright
import os

def run_cuj(page):
    page.goto("http://localhost:8000/frontend/index.html")
    page.wait_for_timeout(2000)

    # Check if online
    status_text = page.locator("#status-text")
    print(f"Status: {status_text.inner_text()}")

    # Type a message
    page.locator("#user-input").fill("Hello, how are you?")
    page.wait_for_timeout(500)
    page.locator("#chat-form button[type='submit']").click()

    # Wait for response
    page.wait_for_timeout(5000)

    # Take screenshot
    page.screenshot(path="/home/jules/verification/screenshots/verification.png")
    page.wait_for_timeout(1000)

if __name__ == "__main__":
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
        context = browser.new_context(
            record_video_dir="/home/jules/verification/videos"
        )
        page = context.new_page()
        try:
            run_cuj(page)
        finally:
            context.close()
            browser.close()
