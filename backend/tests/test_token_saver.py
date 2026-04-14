import pytest
from backend.token_saver import TokenSaver

def test_clean_text():
    ts = TokenSaver()
    assert ts.clean_text("Hello um world") == "Hello world"
    assert ts.clean_text("actually, Hello") == "Hello"
    assert ts.clean_text("Hello... world!!") == "Hello. world!"
    assert ts.clean_text("   too   many   spaces   ") == "too many spaces"
    assert ts.clean_text("basically like you know") == ""

def test_trim_context():
    ts = TokenSaver(max_context_messages=2)
    messages = [{"role": "u", "content": "1"}, {"role": "a", "content": "2"}, {"role": "u", "content": "3"}]
    trimmed = ts.trim_context(messages)
    assert len(trimmed) == 2
    assert trimmed[0]["content"] == "2"

def test_optimize_prompt():
    ts = TokenSaver()
    messages = [{"role": "user", "content": "Hi um there"}]
    prompt = ts.optimize_prompt(messages)
    assert "User: Hi there" in prompt
    assert "AI:" in prompt
