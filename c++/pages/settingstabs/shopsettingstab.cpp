#include "shopsettingstab.h"
#include "helper/thememanager.h"
#include "themes/theme.h"
#include "widgets/settingscard.h"
#include <QVBoxLayout>
#include <QFormLayout>
#include <QLabel>
#include <QLineEdit>
#include <QPushButton>
#include <QMessageBox>
#include <QDoubleValidator>
#include <QHBoxLayout>
#include <QSignalBlocker>
#include <QPixmap>
#include <QSizePolicy>
#include <QPalette>
#include <QFileDialog>
#include <algorithm>
#include <QtMath>

ShopSettingsTab::ShopSettingsTab(ShopSettingDB &sdb, QWidget *parent)
    : ThemeableWidget(parent), shopSettings(sdb)
{
    auto *mainLayout = new QVBoxLayout(this);
    mainLayout->setContentsMargins(24, 24, 24, 24);
    mainLayout->setSpacing(24);

    mainCard = new SettingsCard(this);
    auto *cardLayout = mainCard->contentLayout();

    shopNameEdit = new QLineEdit(mainCard);
    tattooRateEdit = new QLineEdit(mainCard);
    pierceSingleEdit = new QLineEdit(mainCard);
    pierceMultiEdit = new QLineEdit(mainCard);
    sidebarArtworkEdit = new QLineEdit(mainCard);
    sidebarArtworkEdit->setPlaceholderText(tr("Path to sidebar artwork image"));
    sidebarArtworkEdit->setClearButtonEnabled(true);
    sidebarArtworkBrowseBtn = new QPushButton(tr("Browse..."), mainCard);
    saveBtn = new QPushButton(tr("Save Settings"), mainCard);

    tattooRateEdit->setPlaceholderText(tr("e.g. 125.00"));
    pierceSingleEdit->setPlaceholderText(tr("e.g. 60.00"));
    pierceMultiEdit->setPlaceholderText(tr("e.g. 90.00"));

    auto *tattooValidator = new QDoubleValidator(0.0, 1000000.0, 2, this);
    tattooValidator->setNotation(QDoubleValidator::StandardNotation);
    tattooValidator->setLocale(numericLocale);
    tattooRateEdit->setValidator(tattooValidator);

    auto *singleValidator = new QDoubleValidator(0.0, 1000000.0, 2, this);
    singleValidator->setNotation(QDoubleValidator::StandardNotation);
    singleValidator->setLocale(numericLocale);
    pierceSingleEdit->setValidator(singleValidator);

    auto *multiValidator = new QDoubleValidator(0.0, 1000000.0, 2, this);
    multiValidator->setNotation(QDoubleValidator::StandardNotation);
    multiValidator->setLocale(numericLocale);
    pierceMultiEdit->setValidator(multiValidator);

    cardLayout->addWidget(createSectionHeader(":/icons/home.png", tr("Shop Identity")));
    cardLayout->addWidget(createCaptionLabel(tr("Set the name that appears throughout staff and client experiences.")));

    auto *identityLayout = new QVBoxLayout;
    identityLayout->setContentsMargins(0, 0, 0, 0);
    identityLayout->setSpacing(8);
    identityLayout->addWidget(shopNameEdit);
    cardLayout->addLayout(identityLayout);

    cardLayout->addWidget(createSectionHeader(":/icons/stats.png", tr("Service Pricing")));
    cardLayout->addWidget(createCaptionLabel(tr("Adjust tattoo and piercing rates used during scheduling and checkout.")));

    auto *pricingLayout = new QFormLayout;
    pricingLayout->setContentsMargins(0, 0, 0, 0);
    pricingLayout->setHorizontalSpacing(16);
    pricingLayout->setVerticalSpacing(12);
    pricingLayout->addRow(tr("Tattoo / hr"), tattooRateEdit);
    pricingLayout->addRow(tr("Piercing (Single)"), pierceSingleEdit);
    pricingLayout->addRow(tr("Piercing (Multi)"), pierceMultiEdit);
    cardLayout->addLayout(pricingLayout);

    cardLayout->addWidget(createSectionHeader(":/icons/artwork.png", tr("Sidebar Artwork")));
    cardLayout->addWidget(createCaptionLabel(tr("Select the illustration that appears on the dashboard sidebar.")));

    auto *artworkRow = new QHBoxLayout;
    artworkRow->setContentsMargins(0, 0, 0, 0);
    artworkRow->setSpacing(8);
    artworkRow->addWidget(sidebarArtworkEdit);
    artworkRow->addWidget(sidebarArtworkBrowseBtn);
    cardLayout->addLayout(artworkRow);

    sidebarArtworkPreview = new QLabel(mainCard);
    sidebarArtworkPreview->setObjectName(QStringLiteral("sidebarArtworkPreview"));
    sidebarArtworkPreview->setAlignment(Qt::AlignCenter);
    sidebarArtworkPreview->setMinimumHeight(160);
    sidebarArtworkPreview->setWordWrap(true);
    cardLayout->addWidget(sidebarArtworkPreview);

    auto *saveLayout = new QHBoxLayout;
    saveLayout->setContentsMargins(0, 0, 0, 0);
    saveLayout->setSpacing(12);
    saveLayout->addStretch();
    saveLayout->addWidget(saveBtn);
    cardLayout->addLayout(saveLayout);

    secondaryButtons.append(sidebarArtworkBrowseBtn);

    const auto connectChangeSignal = [this](QLineEdit *edit) {
        connect(edit, &QLineEdit::textChanged, this, &ShopSettingsTab::handleFieldChange);
    };
    connectChangeSignal(shopNameEdit);
    connectChangeSignal(tattooRateEdit);
    connectChangeSignal(pierceSingleEdit);
    connectChangeSignal(pierceMultiEdit);
    connect(sidebarArtworkEdit, &QLineEdit::textChanged, this, [this](const QString &value) {
        handleFieldChange(value);
        updateSidebarArtworkPreview(value);
    });

    connect(saveBtn, &QPushButton::clicked, this, [=]() {
        if (shopNameEdit->text().trimmed().isEmpty()) {
            QMessageBox::warning(this, tr("Error"), tr("Shop name cannot be empty!"));
            return;
        }

        double tattooRate = 0.0;
        double singleRate = 0.0;
        double multiRate = 0.0;

        if (!tryParseDouble(tattooRateEdit, tr("Tattoo / hr"), tattooRate) ||
            !tryParseDouble(pierceSingleEdit, tr("Piercing (Single)"), singleRate) ||
            !tryParseDouble(pierceMultiEdit, tr("Piercing (Multi)"), multiRate)) {
            return;
        }

        ShopSettings s = shopSettings.loadSettings();
        s.shopName = shopNameEdit->text().trimmed();
        s.tattooPerHour = tattooRate;
        s.piercingSingle = singleRate;
        s.piercingMulti = multiRate;
        s.sidebarArtworkPath = sidebarArtworkEdit->text().trimmed();

        if (!shopSettings.saveSettings(s)) {
            QMessageBox::critical(this, tr("Save Failed"), tr("Unable to persist shop settings. Please try again."));
            return;
        }

        applyShopNameToAllWindows(s.shopName);
        QMessageBox::information(this, tr("Saved"), tr("Shop settings updated successfully."));
        populateFromSettings(s);
    });
    connect(sidebarArtworkBrowseBtn, &QPushButton::clicked, this, [this]() {
        const QString currentPath = sidebarArtworkEdit ? sidebarArtworkEdit->text() : QString();
        const QString path = QFileDialog::getOpenFileName(this, tr("Select Sidebar Artwork"), currentPath, tr("Images (*.png *.jpg *.jpeg *.bmp *.gif)"));
        if (!path.isEmpty() && sidebarArtworkEdit) {
            sidebarArtworkEdit->setText(path);
        }
    });

    mainLayout->addWidget(mainCard);
    mainLayout->addStretch();

    reloadSettings();
    updateTheme();
}

void ShopSettingsTab::updateTheme()
{
    Theme t = ThemeManager::instance()->currentTheme();
    const QColor defaultText = palette().color(QPalette::Text);
    const QColor textColor = t.textColor.isValid() ? t.textColor : (t.text.isValid() ? t.text : defaultText);
    QColor mutedText = textColor;
    mutedText.setAlphaF(0.75);
    const QColor accent = t.primary.isValid() ? t.primary : (t.accent.isValid() ? t.accent : textColor);
    const QColor fieldBg = t.background.isValid() ? t.background.lighter(110) : palette().color(QPalette::Base);
    const QColor border = t.borderColor.isValid() ? t.borderColor : (t.border.isValid() ? t.border : accent.darker(115));
    const QColor hoverBg = fieldBg.lighter(108);

    for (QLabel *heading : headingLabels) {
        heading->setStyleSheet(QStringLiteral("color: %1; font-weight: 600;").arg(textColor.name()));
    }

    for (QLabel *caption : captionLabels) {
        caption->setStyleSheet(QStringLiteral("color: %1;").arg(mutedText.name(QColor::HexArgb)));
    }

    const QString fieldStyle = QStringLiteral(
        "QLineEdit {"
        "    background-color: %1;"
        "    border: 1px solid %2;"
        "    border-radius: 10px;"
        "    padding: 6px 10px;"
        "    color: %3;"
        "}"
        "QLineEdit:focus {"
        "    border-color: %4;"
        "}")
        .arg(fieldBg.name(QColor::HexArgb))
        .arg(border.name())
        .arg(textColor.name())
        .arg(accent.name());

    shopNameEdit->setStyleSheet(fieldStyle);
    tattooRateEdit->setStyleSheet(fieldStyle);
    pierceSingleEdit->setStyleSheet(fieldStyle);
    pierceMultiEdit->setStyleSheet(fieldStyle);
    sidebarArtworkEdit->setStyleSheet(fieldStyle);

    for (QPushButton *secondary : secondaryButtons) {
        secondary->setStyleSheet(QStringLiteral(
            "QPushButton {"
            "    background-color: %1;"
            "    color: %2;"
            "    border-radius: 10px;"
            "    padding: 8px 16px;"
            "    border: 1px solid %3;"
            "}"
            "QPushButton:hover {"
            "    background-color: %4;"
            "}")
            .arg(fieldBg.name(QColor::HexArgb))
            .arg(textColor.name())
            .arg(border.name())
            .arg(hoverBg.name(QColor::HexArgb)));
    }

    styleButton(saveBtn);
    if (sidebarArtworkPreview) {
        sidebarArtworkPreview->setStyleSheet(QStringLiteral(
            "#sidebarArtworkPreview {"
            "    border: 1px dashed %1;"
            "    border-radius: 12px;"
            "    background-color: %2;"
            "    color: %3;"
            "    padding: 12px;"
            "}"
        ).arg(border.name())
         .arg(fieldBg.name(QColor::HexArgb))
         .arg(textColor.name()));
    }
    if (mainCard) {
        mainCard->updateTheme();
    }
}

QWidget *ShopSettingsTab::createSectionHeader(const QString &iconPath, const QString &title)
{
    QWidget *header = new QWidget(this);
    auto *layout = new QHBoxLayout(header);
    layout->setContentsMargins(0, 0, 0, 0);
    layout->setSpacing(8);

    auto *iconLabel = new QLabel(header);
    iconLabel->setPixmap(QPixmap(iconPath).scaled(24, 24, Qt::KeepAspectRatio, Qt::SmoothTransformation));

    auto *titleLabel = new QLabel(title, header);
    QFont font = titleLabel->font();
    font.setPointSize(font.pointSize() + 2);
    font.setBold(true);
    titleLabel->setFont(font);
    titleLabel->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Preferred);

    layout->addWidget(iconLabel);
    layout->addWidget(titleLabel);
    layout->addStretch();

    headingLabels.append(titleLabel);
    return header;
}

QLabel *ShopSettingsTab::createCaptionLabel(const QString &text)
{
    auto *caption = new QLabel(text, this);
    caption->setWordWrap(true);
    caption->setObjectName("sectionCaption");
    QFont font = caption->font();
    const int pointSize = font.pointSize();
    if (pointSize > 0) {
        font.setPointSize(std::max(9, pointSize - 1));
    }
    caption->setFont(font);
    captionLabels.append(caption);
    return caption;
}

void ShopSettingsTab::updateSidebarArtworkPreview(const QString &path)
{
    if (!sidebarArtworkPreview) {
        return;
    }

    const QString trimmed = path.trimmed();
    if (!trimmed.isEmpty()) {
        QPixmap preview(trimmed);
        if (!preview.isNull()) {
            sidebarArtworkPreview->setPixmap(preview.scaled(200, 200, Qt::KeepAspectRatio, Qt::SmoothTransformation));
            sidebarArtworkPreview->setText(QString());
            return;
        }
    }

    sidebarArtworkPreview->setPixmap(QPixmap());
    sidebarArtworkPreview->setText(tr("No artwork selected"));
}

void ShopSettingsTab::applyShopNameToAllWindows(const QString &shopName)
{
    // This function will need to be adjusted as it can't directly set the window title of the application
    // A signal/slot mechanism to the main window would be a good approach.
    // For now, we'll just log it.
    qDebug() << "Shop name changed to:" << shopName;
}

void ShopSettingsTab::populateFromSettings(const ShopSettings &settings)
{
    const QString trimmedName = settings.shopName.trimmed();
    const bool hasData = !trimmedName.isEmpty() ||
        !qFuzzyIsNull(settings.tattooPerHour) ||
        !qFuzzyIsNull(settings.piercingSingle) ||
        !qFuzzyIsNull(settings.piercingMulti) ||
        !settings.sidebarArtworkPath.trimmed().isEmpty();

    hasPersistedSettings = hasData;
    lastLoadedSettings = settings;

    const QSignalBlocker blocker1(shopNameEdit);
    const QSignalBlocker blocker2(tattooRateEdit);
    const QSignalBlocker blocker3(pierceSingleEdit);
    const QSignalBlocker blocker4(pierceMultiEdit);
    const QSignalBlocker blocker5(sidebarArtworkEdit);

    isPopulatingFields = true;

    if (hasData) {
        shopNameEdit->setText(trimmedName);
        tattooRateEdit->setText(QString::number(settings.tattooPerHour, 'f', 2));
        pierceSingleEdit->setText(QString::number(settings.piercingSingle, 'f', 2));
        pierceMultiEdit->setText(QString::number(settings.piercingMulti, 'f', 2));
        sidebarArtworkEdit->setText(settings.sidebarArtworkPath);
    } else {
        shopNameEdit->clear();
        tattooRateEdit->clear();
        pierceSingleEdit->clear();
        pierceMultiEdit->clear();
        sidebarArtworkEdit->clear();
    }

    isPopulatingFields = false;
    updateSidebarArtworkPreview(sidebarArtworkEdit->text());
    updateButtonStates();
}

bool ShopSettingsTab::tryParseDouble(QLineEdit *lineEdit, const QString &fieldLabel, double &value)
{
    bool ok = false;
    value = numericLocale.toDouble(lineEdit->text().trimmed(), &ok);
    if (!ok) {
        QMessageBox::warning(this, tr("Invalid Value"),
                             tr("%1 must be a valid number.").arg(fieldLabel));
        lineEdit->setFocus();
        lineEdit->selectAll();
        return false;
    }
    return true;
}

void ShopSettingsTab::reloadSettings()
{
    populateFromSettings(shopSettings.loadSettings());
}

void ShopSettingsTab::handleFieldChange(const QString &)
{
    if (isPopulatingFields) {
        return;
    }
    updateButtonStates();
}

void ShopSettingsTab::updateButtonStates()
{
    const bool nameIsValid = !shopNameEdit->text().trimmed().isEmpty();
    const bool numericValid = ratesAreValid();
    const bool dirty = hasChanges();

    saveBtn->setEnabled(nameIsValid && numericValid && dirty);
}

bool ShopSettingsTab::hasChanges() const
{
    auto doublesEqual = [](double a, double b) {
        return qFuzzyCompare(1 + a, 1 + b);
    };

    const QString trimmedName = shopNameEdit->text().trimmed();

    if (!hasPersistedSettings) {
        return !trimmedName.isEmpty() ||
            !tattooRateEdit->text().trimmed().isEmpty() ||
            !pierceSingleEdit->text().trimmed().isEmpty() ||
            !pierceMultiEdit->text().trimmed().isEmpty() ||
            !sidebarArtworkEdit->text().trimmed().isEmpty();
    }

    if (QString::compare(trimmedName, lastLoadedSettings.shopName.trimmed(), Qt::CaseSensitive) != 0) {
        return true;
    }

    bool ok = false;
    const QString tattooText = tattooRateEdit->text().trimmed();
    const QString tattooPersisted = QString::number(lastLoadedSettings.tattooPerHour, 'f', 2);
    const double tattoo = numericLocale.toDouble(tattooText, &ok);
    if ((ok && !doublesEqual(tattoo, lastLoadedSettings.tattooPerHour)) || (!ok && tattooText != tattooPersisted)) {
        return true;
    }

    ok = false;
    const QString singleText = pierceSingleEdit->text().trimmed();
    const QString singlePersisted = QString::number(lastLoadedSettings.piercingSingle, 'f', 2);
    const double single = numericLocale.toDouble(singleText, &ok);
    if ((ok && !doublesEqual(single, lastLoadedSettings.piercingSingle)) || (!ok && singleText != singlePersisted)) {
        return true;
    }

    ok = false;
    const QString multiText = pierceMultiEdit->text().trimmed();
    const QString multiPersisted = QString::number(lastLoadedSettings.piercingMulti, 'f', 2);
    const double multi = numericLocale.toDouble(multiText, &ok);
    if ((ok && !doublesEqual(multi, lastLoadedSettings.piercingMulti)) || (!ok && multiText != multiPersisted)) {
        return true;
    }

    const QString artworkPath = sidebarArtworkEdit->text().trimmed();
    if (QString::compare(artworkPath, lastLoadedSettings.sidebarArtworkPath.trimmed(), Qt::CaseSensitive) != 0) {
        return true;
    }

    return false;
}

bool ShopSettingsTab::ratesAreValid() const
{
    auto isValidDouble = [this](QLineEdit *lineEdit) {
        bool ok = false;
        numericLocale.toDouble(lineEdit->text().trimmed(), &ok);
        return ok;
    };

    return isValidDouble(tattooRateEdit) &&
        isValidDouble(pierceSingleEdit) &&
        isValidDouble(pierceMultiEdit);
}
