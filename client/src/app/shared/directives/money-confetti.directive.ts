import { Directive, inject } from '@angular/core';
import { ProgressBar } from 'primeng/progressbar';

@Directive({
  selector: 'p-progressBar[pbaMoneyConfetti]',
  standalone: true,
  host: {
    '(click)': 'onClick($event)',
  },
})
export class MoneyConfettiDirective {
  #progressBar = inject(ProgressBar);

  onClick(event: MouseEvent): void {
    const progressValue = this.#progressBar.value ?? 0;
    const confettiCount = (20 * progressValue) / 100;
    if (confettiCount <= 0) {
      return;
    }

    // create necessary HTML elements
    this.createStyleElement();
    const container = this.createContainerElement(event.pageX, event.pageY);
    this.createConfettiElements(container, confettiCount);

    // attach elements to the DOM and remove them after some delay
    document.body.appendChild(container);
    setTimeout(() => container.remove(), 700);
  }

  private createConfettiElements(container: HTMLDivElement, confettiCount: number) {
    for (let i = 0; i < confettiCount; i++) {
      const styles = `
        transform: translate3d(${this.random(500) - 250}px, ${this.random(200) - 150}px, 0) rotate(${this.random(360)}deg);
      `;
      const confettiEl = document.createElement('div');
      confettiEl.textContent = '💵';
      confettiEl.style.cssText = styles;
      confettiEl.classList.add('confetti');
      container.appendChild(confettiEl);
    }
  }

  private createContainerElement(xPos: number, yPos: number) {
    const container = document.createElement('div');
    container.style.position = 'absolute';
    container.style.left = `${xPos}px`;
    container.style.top = `${yPos}px`;
    return container;
  }

  private createStyleElement(): void {
    let styleEl = document.getElementById('confetti-style') as HTMLStyleElement;
    if (styleEl) {
      // our style element is already attached to the DOM, no need to create it
      return;
    }

    const headEl = document.getElementsByTagName('head')[0];
    styleEl = document.createElement('style');
    styleEl.id = 'confetti-style';

    const css = `
      @keyframes bang {
        from {
          transform: translate3d(0,0,0);
          opacity: 1;
        }
      }

      .confetti {
        position: absolute;
        opacity: 0;
        pointer-events: none;
        animation: bang 700ms ease-out forwards;
      }
    `;
    styleEl.appendChild(document.createTextNode(css));
    headEl.appendChild(styleEl);
  }

  private random(max: number): number {
    return Math.random() * max;
  }
}
