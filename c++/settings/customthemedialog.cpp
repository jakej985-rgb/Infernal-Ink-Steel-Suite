#include "customthemedialog.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QColorDialog>
#include <QLabel>
#include "helper/thememanager.h"

CustomThemeDialog::CustomThemeDialog(QWidget *parent)
    : ThemeableDialog(parent)
{
    setWindowTitle("Create Custom Theme");

    QVBoxLayout *mainLayout = new QVBoxLayout(this);

    nameEdit = new QLineEdit(this);
    nameEdit->setPlaceholderText("Theme Name");
    mainLayout->addWidget(nameEdit);

    backgroundColorBtn = new QPushButton("Background Color", this);
    textColorBtn = new QPushButton("Text Color", this);
    buttonColorBtn = new QPushButton("Button Color", this);
    buttonHoverColorBtn = new QPushButton("Button Hover Color", this);
    buttonTextColorBtn = new QPushButton("Button Text Color", this);
    borderColorBtn = new QPushButton("Border Color", this);

    previewLabel = new QLabel("AaBbCc Preview", this);
    previewLabel->setAlignment(Qt::AlignCenter);
    previewLabel->setFixedHeight(100);
    previewLabel->setFrameStyle(QFrame::Panel | QFrame::Sunken);

    okBtn = new QPushButton("OK", this);
    cancelBtn = new QPushButton("Cancel", this);

    mainLayout->addWidget(backgroundColorBtn);
    mainLayout->addWidget(textColorBtn);
    mainLayout->addWidget(buttonColorBtn);
    mainLayout->addWidget(buttonHoverColorBtn);
    mainLayout->addWidget(buttonTextColorBtn);
    mainLayout->addWidget(borderColorBtn);
    mainLayout->addWidget(previewLabel);

    QHBoxLayout *btnLayout = new QHBoxLayout;
    btnLayout->addWidget(okBtn);
    btnLayout->addWidget(cancelBtn);
    mainLayout->addLayout(btnLayout);

    // Connections
    connect(backgroundColorBtn, &QPushButton::clicked, this, &CustomThemeDialog::chooseBackgroundColor);
    connect(textColorBtn, &QPushButton::clicked, this, &CustomThemeDialog::chooseTextColor);
    connect(buttonColorBtn, &QPushButton::clicked, this, &CustomThemeDialog::chooseButtonColor);
    connect(buttonHoverColorBtn, &QPushButton::clicked, this, &CustomThemeDialog::chooseButtonHoverColor);
    connect(buttonTextColorBtn, &QPushButton::clicked, this, &CustomThemeDialog::chooseButtonTextColor);
    connect(borderColorBtn, &QPushButton::clicked, this, &CustomThemeDialog::chooseBorderColor);
    connect(okBtn, &QPushButton::clicked, this, &QDialog::accept);
    connect(cancelBtn, &QPushButton::clicked, this, &QDialog::reject);

    // Default theme for preview
    currentTheme.name = "Preview";
    currentTheme.backgroundColor = QColor("#ffffff");
    currentTheme.textColor = QColor("#000000");
    currentTheme.buttonColor = QColor("#0077ff");
    currentTheme.buttonHoverColor = QColor("#005bb5");
    currentTheme.buttonTextColor = QColor("#ffffff");
    currentTheme.borderColor = QColor("#0077ff");
    updatePreview();
}

void CustomThemeDialog::chooseBackgroundColor() {
    QColor c = QColorDialog::getColor(currentTheme.backgroundColor, this);
    if (c.isValid()) currentTheme.backgroundColor = c;
    updatePreview();
}

void CustomThemeDialog::chooseTextColor() {
    QColor c = QColorDialog::getColor(currentTheme.textColor, this);
    if (c.isValid()) currentTheme.textColor = c;
    updatePreview();
}

void CustomThemeDialog::chooseButtonColor() {
    QColor c = QColorDialog::getColor(currentTheme.buttonColor, this);
    if (c.isValid()) currentTheme.buttonColor = c;
    updatePreview();
}

void CustomThemeDialog::chooseButtonHoverColor() {
    QColor c = QColorDialog::getColor(currentTheme.buttonHoverColor, this);
    if (c.isValid()) currentTheme.buttonHoverColor = c;
    updatePreview();
}

void CustomThemeDialog::chooseButtonTextColor() {
    QColor c = QColorDialog::getColor(currentTheme.buttonTextColor, this);
    if (c.isValid()) currentTheme.buttonTextColor = c;
    updatePreview();
}

void CustomThemeDialog::chooseBorderColor() {
    QColor c = QColorDialog::getColor(currentTheme.borderColor, this);
    if (c.isValid()) currentTheme.borderColor = c;
    updatePreview();
}

void CustomThemeDialog::updatePreview() {
    QPalette pal = previewLabel->palette();
    pal.setColor(QPalette::Window, currentTheme.backgroundColor);
    pal.setColor(QPalette::WindowText, currentTheme.textColor);
    previewLabel->setAutoFillBackground(true);
    previewLabel->setPalette(pal);
}

Theme CustomThemeDialog::theme() {
    currentTheme.name = nameEdit->text().trimmed();
    return currentTheme;
}

void CustomThemeDialog::updateTheme() {
    Theme t = ThemeManager::instance()->currentTheme();
    setStyleSheet(QString("background-color:%1; color:%2;").arg(t.background.name()).arg(t.text.name()));
    nameEdit->setStyleSheet(QString("border: 1px solid %1;").arg(t.primary.name()));
    styleButton(backgroundColorBtn);
    styleButton(textColorBtn);
    styleButton(buttonColorBtn);
    styleButton(buttonHoverColorBtn);
    styleButton(buttonTextColorBtn);
    styleButton(borderColorBtn);
    styleButton(okBtn);
    styleButton(cancelBtn);
}
