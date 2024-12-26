// ボタンのクリックイベントを設定
document.addEventListener("DOMContentLoaded", () => {
    const button = document.getElementById("myButton");
    button.addEventListener("click", () => {
        const now = new Date();
        console.log(`${now.toLocaleString()}：ボタンを押しました！`);
    });
});
