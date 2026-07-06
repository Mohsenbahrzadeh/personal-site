(function () {
  const BUTTONS = [
    { cmd: 'bold', label: '<b>B</b>', title: 'ضخیم' },
    { cmd: 'italic', label: '<i>I</i>', title: 'کج' },
    { cmd: 'underline', label: '<u>U</u>', title: 'زیرخط' },
    { cmd: 'formatBlock', arg: 'H2', label: 'H2', title: 'تیتر بزرگ' },
    { cmd: 'formatBlock', arg: 'H3', label: 'H3', title: 'تیتر کوچک' },
    { cmd: 'formatBlock', arg: 'P', label: '¶', title: 'پاراگراف عادی' },
    { cmd: 'insertUnorderedList', label: '•', title: 'لیست نقطه‌ای' },
    { cmd: 'insertOrderedList', label: '1.', title: 'لیست شماره‌دار' },
    { cmd: 'formatBlock', arg: 'BLOCKQUOTE', label: '"', title: 'نقل‌قول' },
    { cmd: 'createLink', label: '🔗', title: 'افزودن لینک', prompt: 'آدرس لینک را وارد کنید:' },
    { cmd: 'removeFormat', label: '✕', title: 'حذف قالب‌بندی' }
  ];

  function initRichEditor(textareaId) {
    const textarea = document.getElementById(textareaId);
    if (!textarea) return;

    const wrap = document.createElement('div');
    wrap.className = 'rte-wrap';

    const toolbar = document.createElement('div');
    toolbar.className = 'rte-toolbar';

    const editor = document.createElement('div');
    editor.className = 'rte-editor';
    editor.contentEditable = 'true';
    editor.innerHTML = textarea.value;

    function sync() {
      textarea.value = editor.innerHTML;
    }

    BUTTONS.forEach(b => {
      const btn = document.createElement('button');
      btn.type = 'button';
      btn.className = 'rte-btn';
      btn.innerHTML = b.label;
      btn.title = b.title;
      btn.addEventListener('click', () => {
        editor.focus();
        if (b.prompt) {
          const url = window.prompt(b.prompt, 'https://');
          if (url) document.execCommand(b.cmd, false, url);
        } else {
          document.execCommand(b.cmd, false, b.arg || null);
        }
        sync();
      });
      toolbar.appendChild(btn);
    });

    editor.addEventListener('input', sync);
    editor.addEventListener('blur', sync);

    textarea.style.display = 'none';
    wrap.appendChild(toolbar);
    wrap.appendChild(editor);
    textarea.parentNode.insertBefore(wrap, textarea);

    const form = textarea.closest('form');
    if (form) form.addEventListener('submit', sync);
  }

  document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('[data-rich-editor]').forEach(el => initRichEditor(el.id));
  });
})();
