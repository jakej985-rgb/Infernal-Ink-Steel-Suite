#include "admintab.h"
#include "helper/thememanager.h"
#include "themes/theme.h"
#include "widgets/settingscard.h"
#include "widgets/collapsiblesection.h"
#include "pages/settingstabs/shopsettingstab.h"
#include "dialog/createuserdialog.h"

#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QFormLayout>
#include <QLabel>
#include <QLineEdit>
#include <QPushButton>
#include <QComboBox>
#include <QTableWidget>
#include <QHeaderView>
#include <QFileDialog>
#include <QColorDialog>
#include <QColor>
#include <QMessageBox>
#include <QPalette>
#include <QScrollArea>
#include <QSizePolicy>
#include <QAbstractItemView>
#include <QWidget>
#include <QInputDialog>
#include <QFontComboBox>
#include <QFont>
#include <QFrame>
#include <QFileInfo>
#include <QUrl>
#include <QHash>
#include <algorithm>

namespace {
static QString normalizedColorString(const QString &value)
{
    QColor color(value);
    if (!color.isValid()) {
        return QString();
    }
    if (color.alpha() == 255) {
        return color.name();
    }
    return color.name(QColor::HexArgb);
}
}

AdminTab::AdminTab(UserDB &udb, ShopSettingDB &sdb, QWidget *parent)
    : ThemeableWidget(parent)
    , userDB(udb)
    , shopSettingsDB(sdb)
{
    auto *mainLayout = new QVBoxLayout(this);
    mainLayout->setContentsMargins(24, 24, 24, 24);
    mainLayout->setSpacing(24);

    auto *scrollArea = new QScrollArea(this);
    scrollArea->setWidgetResizable(true);
    scrollArea->setFrameShape(QFrame::NoFrame);
    scrollArea->setHorizontalScrollBarPolicy(Qt::ScrollBarAlwaysOff);

    auto *scrollWidget = new QWidget(scrollArea);
    auto *scrollLayout = new QVBoxLayout(scrollWidget);
    scrollLayout->setContentsMargins(0, 0, 0, 0);
    scrollLayout->setSpacing(24);

    adminCard = new SettingsCard(scrollWidget);
    auto *cardLayout = adminCard->contentLayout();
    cardLayout->setSpacing(12);

    initializeUserSection();
    initializeShopSection();
    initializeLoginSection();

    if (userSection) {
        cardLayout->addWidget(userSection);
    }
    if (shopSection) {
        cardLayout->addWidget(shopSection);
    }
    if (loginSection) {
        cardLayout->addWidget(loginSection);
    }

    scrollLayout->addWidget(adminCard);
    scrollLayout->addStretch();
    scrollArea->setWidget(scrollWidget);

    mainLayout->addWidget(scrollArea);

    reloadUsers();
    loadLoginBranding();
    updateTheme();
}

void AdminTab::initializeUserSection()
{
    userSection = new CollapsibleSection(tr("Admin User Controls"), QIcon(":/icons/clients.png"), adminCard);
    userSection->setCaption(tr("Invite teammates, manage permissions, and reset credentials from one place."));

    auto *layout = userSection->contentLayout();
    layout->setSpacing(16);

    roleOptions = {
        {tr("Admin"), QStringLiteral("Admin")},
        {tr("Manager"), QStringLiteral("Manager")},
        {tr("User"), QStringLiteral("User")}
    };

    addUserBtn = new QPushButton(tr("Create User"), userSection);
    connect(addUserBtn, &QPushButton::clicked, this, &AdminTab::handleAddUser);

    userTable = new QTableWidget(userSection);
    userTable->setColumnCount(2);
    userTable->setHorizontalHeaderLabels({tr("Username"), tr("Role")});
    userTable->horizontalHeader()->setStretchLastSection(true);
    userTable->horizontalHeader()->setSectionResizeMode(QHeaderView::Stretch);
    userTable->verticalHeader()->setVisible(false);
    userTable->setSelectionMode(QAbstractItemView::SingleSelection);
    userTable->setSelectionBehavior(QAbstractItemView::SelectRows);
    userTable->setEditTriggers(QAbstractItemView::NoEditTriggers);
    userTable->setAlternatingRowColors(true);
    userTable->setMinimumHeight(160);
    layout->addWidget(userTable);

    connect(userTable, &QTableWidget::currentCellChanged, this, [this](int currentRow, int, int, int) {
        onUserSelectionChanged(currentRow);
    });

    selectedUserLabel = new QLabel(tr("Select a user"), userSection);
    selectedUserLabel->setWordWrap(true);
    layout->addWidget(selectedUserLabel);

    updateRoleBtn = new QPushButton(tr("Update Role"), userSection);
    resetPasswordBtn = new QPushButton(tr("Reset Password"), userSection);
    updateRoleBtn->setEnabled(false);
    resetPasswordBtn->setEnabled(false);

    connect(updateRoleBtn, &QPushButton::clicked, this, &AdminTab::handleRoleUpdate);
    connect(resetPasswordBtn, &QPushButton::clicked, this, &AdminTab::handlePasswordReset);

    auto *manageButtons = new QHBoxLayout;
    manageButtons->setContentsMargins(0, 0, 0, 0);
    manageButtons->setSpacing(12);
    manageButtons->addStretch();
    manageButtons->addWidget(addUserBtn);
    manageButtons->addWidget(updateRoleBtn);
    manageButtons->addWidget(resetPasswordBtn);
    layout->addLayout(manageButtons);
}

void AdminTab::initializeShopSection()
{
    shopSection = new CollapsibleSection(tr("Shop Settings"), QIcon(":/icons/home.png"), adminCard);
    shopSection->setCaption(tr("Keep your brand and pricing consistent across the application."));
    shopSection->setExpanded(false);

    auto *layout = shopSection->contentLayout();
    embeddedShopSettings = new ShopSettingsTab(shopSettingsDB, shopSection);
    layout->addWidget(embeddedShopSettings);
}

void AdminTab::initializeLoginSection()
{
    loginSection = new CollapsibleSection(tr("Login Screen"), QIcon(":/icons/login.png"), adminCard);
    loginSection->setCaption(tr("Tailor the welcome experience with messaging, accent colors, and artwork."));
    loginSection->setExpanded(false);

    accentPresets = {
        {tr("Ocean Mist"), QStringLiteral("#3A6EA5")},
        {tr("Electric Plum"), QStringLiteral("#7F5AF0")},
        {tr("Sunset Ember"), QStringLiteral("#F97316")},
        {tr("Emerald Fade"), QStringLiteral("#10B981")},
        {tr("Midnight Rose"), QStringLiteral("#FF4D6D")}
    };
    if (!accentPresets.isEmpty()) {
        customAccentColor = accentPresets.first().second;
    }

    auto *layout = loginSection->contentLayout();

    headlineEdit = new QLineEdit(loginSection);
    headlineEdit->setPlaceholderText(tr("Welcome headline"));
    taglineEdit = new QLineEdit(loginSection);
    taglineEdit->setPlaceholderText(tr("Supporting tagline"));
    backgroundEdit = new QLineEdit(loginSection);
    backgroundEdit->setPlaceholderText(tr("Path to background image"));

    textFields.append(headlineEdit);
    textFields.append(taglineEdit);
    textFields.append(backgroundEdit);

    accentPresetCombo = new QComboBox(loginSection);
    for (const auto &preset : accentPresets) {
        accentPresetCombo->addItem(preset.first, preset.second);
    }
    accentPresetCombo->addItem(tr("Custom Accent"), QStringLiteral("custom"));

    headlineFontCombo = new QFontComboBox(loginSection);
    taglineFontCombo = new QFontComboBox(loginSection);

    comboFields.append(accentPresetCombo);
    comboFields.append(headlineFontCombo);
    comboFields.append(taglineFontCombo);

    accentPickerBtn = new QPushButton(tr("Choose Color"), loginSection);
    accentPickerBtn->setEnabled(false);
    backgroundBrowseBtn = new QPushButton(tr("Browse..."), loginSection);
    textColorBtn = new QPushButton(tr("Text Color"), loginSection);

    secondaryButtons.append(accentPickerBtn);
    secondaryButtons.append(backgroundBrowseBtn);
    secondaryButtons.append(textColorBtn);

    auto *brandingForm = new QFormLayout;
    brandingForm->setContentsMargins(0, 0, 0, 0);
    brandingForm->setHorizontalSpacing(16);
    brandingForm->setVerticalSpacing(12);
    brandingForm->addRow(tr("Headline"), headlineEdit);
    brandingForm->addRow(tr("Tagline"), taglineEdit);

    auto makeComboRow = [](QWidget *first, QWidget *second, QWidget *parent) {
        auto *rowWidget = new QWidget(parent);
        auto *rowLayout = new QHBoxLayout(rowWidget);
        rowLayout->setContentsMargins(0, 0, 0, 0);
        rowLayout->setSpacing(8);
        rowLayout->addWidget(first);
        rowLayout->addWidget(second);
        return rowWidget;
    };

    brandingForm->addRow(tr("Accent Theme"), makeComboRow(accentPresetCombo, accentPickerBtn, loginSection));
    brandingForm->addRow(tr("Background"), makeComboRow(backgroundEdit, backgroundBrowseBtn, loginSection));
    brandingForm->addRow(tr("Headline Font"), headlineFontCombo);
    brandingForm->addRow(tr("Tagline Font"), taglineFontCombo);
    brandingForm->addRow(tr("Text Color"), textColorBtn);

    layout->addLayout(brandingForm);

    previewFrame = new QFrame(loginSection);
    previewFrame->setObjectName(QStringLiteral("loginPreviewFrame"));
    previewFrame->setMinimumHeight(200);

    auto *previewLayout = new QVBoxLayout(previewFrame);
    previewLayout->setContentsMargins(24, 24, 24, 24);
    previewLayout->setSpacing(12);

    previewHeadlineLabel = new QLabel(tr("Let's get inking"), previewFrame);
    previewHeadlineLabel->setAlignment(Qt::AlignCenter);
    previewHeadlineLabel->setWordWrap(true);
    previewTaglineLabel = new QLabel(tr("Welcome back"), previewFrame);
    previewTaglineLabel->setAlignment(Qt::AlignCenter);
    previewTaglineLabel->setWordWrap(true);
    previewStatusLabel = new QLabel(QString(), previewFrame);
    previewStatusLabel->setAlignment(Qt::AlignCenter);
    previewStatusLabel->setWordWrap(true);

    previewLayout->addStretch();
    previewLayout->addWidget(previewHeadlineLabel, 0, Qt::AlignCenter);
    previewLayout->addWidget(previewTaglineLabel, 0, Qt::AlignCenter);
    previewLayout->addStretch();
    previewLayout->addWidget(previewStatusLabel, 0, Qt::AlignCenter);

    layout->addWidget(previewFrame);

    saveBrandingBtn = new QPushButton(tr("Save Branding"), loginSection);

    connect(saveBrandingBtn, &QPushButton::clicked, this, &AdminTab::saveLoginBranding);
    connect(backgroundBrowseBtn, &QPushButton::clicked, this, &AdminTab::browseForBackground);
    connect(accentPickerBtn, &QPushButton::clicked, this, &AdminTab::pickAccentColor);
    connect(accentPresetCombo, &QComboBox::currentIndexChanged, this, &AdminTab::handleAccentPresetChanged);
    connect(textColorBtn, &QPushButton::clicked, this, &AdminTab::openTextColorDialog);
    connect(headlineEdit, &QLineEdit::textChanged, this, &AdminTab::updateLoginPreview);
    connect(taglineEdit, &QLineEdit::textChanged, this, &AdminTab::updateLoginPreview);
    connect(backgroundEdit, &QLineEdit::textChanged, this, &AdminTab::updateLoginPreview);
    connect(headlineFontCombo, &QFontComboBox::currentFontChanged, this, [this](const QFont &) {
        updateLoginPreview();
    });
    connect(taglineFontCombo, &QFontComboBox::currentFontChanged, this, [this](const QFont &) {
        updateLoginPreview();
    });

    auto *saveLayout = new QHBoxLayout;
    saveLayout->setContentsMargins(0, 0, 0, 0);
    saveLayout->addStretch();
    saveLayout->addWidget(saveBrandingBtn);
    layout->addLayout(saveLayout);

    handleAccentPresetChanged(accentPresetCombo->currentIndex());
    updateTextColorButton();
    updateLoginPreview();
}

void AdminTab::reloadUsers()
{
    cachedUsers = userDB.getAllUsers();

    if (!userTable) {
        return;
    }

    userTable->setRowCount(cachedUsers.size());
    int row = 0;
    for (const auto &user : cachedUsers) {
        auto *nameItem = new QTableWidgetItem(user.username);
        nameItem->setFlags(Qt::ItemIsSelectable | Qt::ItemIsEnabled);
        auto *roleItem = new QTableWidgetItem(user.role);
        roleItem->setFlags(Qt::ItemIsSelectable | Qt::ItemIsEnabled);
        userTable->setItem(row, 0, nameItem);
        userTable->setItem(row, 1, roleItem);
        ++row;
    }

    if (!cachedUsers.isEmpty()) {
        userTable->selectRow(0);
        onUserSelectionChanged(0);
    } else {
        onUserSelectionChanged(-1);
    }
}

void AdminTab::onUserSelectionChanged(int currentRow)
{
    const bool validRow = currentRow >= 0 && currentRow < cachedUsers.size();

    if (!selectedUserLabel) {
        return;
    }

    if (!validRow) {
        selectedUserLabel->setText(tr("Select a user"));
        updateRoleBtn->setEnabled(false);
        resetPasswordBtn->setEnabled(false);
        return;
    }

    const User &user = cachedUsers.at(currentRow);
    selectedUserLabel->setText(tr("Selected: %1 (%2)").arg(user.username, user.role));
    updateRoleBtn->setEnabled(true);
    resetPasswordBtn->setEnabled(true);
}

void AdminTab::handleAddUser()
{
    CreateUserDialog dialog(roleOptions, this);
    if (dialog.exec() != QDialog::Accepted) {
        return;
    }

    const QString username = dialog.username();
    const QString password = dialog.password();
    const QString role = dialog.role();

    if (userDB.addUser(username, password, role.isEmpty() ? QStringLiteral("User") : role)) {
        showMessage(tr("User Added"), tr("The team member has been created."), QMessageBox::Information);
        reloadUsers();
    } else {
        showMessage(tr("Unable to Add User"), tr("The username already exists or the account could not be saved."), QMessageBox::Critical);
    }
}

void AdminTab::handleRoleUpdate()
{
    if (!userTable) {
        return;
    }
    int row = userTable->currentRow();
    if (row < 0 || row >= cachedUsers.size()) {
        showMessage(tr("Select a User"), tr("Choose a user from the list before updating roles."), QMessageBox::Warning);
        return;
    }

    User user = cachedUsers.at(row);

    const QVector<QPair<QString, QString>> options = roleOptions.isEmpty()
        ? QVector<QPair<QString, QString>>{{tr("User"), QStringLiteral("User")}}
        : roleOptions;

    QStringList roleLabels;
    QHash<QString, QString> labelToValue;
    int currentIndex = 0;
    for (int i = 0; i < options.size(); ++i) {
        const auto &option = options.at(i);
        roleLabels.append(option.first);
        labelToValue.insert(option.first, option.second);
        if (option.second.compare(user.role, Qt::CaseInsensitive) == 0) {
            currentIndex = i;
        }
    }

    bool accepted = false;
    const QString chosenLabel = QInputDialog::getItem(
        this,
        tr("Update Role"),
        tr("Select the new role for %1:").arg(user.username),
        roleLabels,
        currentIndex,
        false,
        &accepted
    );

    if (!accepted) {
        return;
    }

    const QString roleValue = labelToValue.value(chosenLabel, user.role);
    if (roleValue == user.role) {
        showMessage(tr("No Changes"), tr("Select a different role to update."), QMessageBox::Information);
        return;
    }

    if (userDB.updateRole(user.username, roleValue)) {
        showMessage(tr("Role Updated"), tr("%1 is now a %2.").arg(user.username, roleValue), QMessageBox::Information);
        reloadUsers();
    } else {
        showMessage(tr("Update Failed"), tr("Unable to change this user's role."), QMessageBox::Critical);
    }
}

void AdminTab::handlePasswordReset()
{
    if (!userTable) {
        return;
    }
    int row = userTable->currentRow();
    if (row < 0 || row >= cachedUsers.size()) {
        showMessage(tr("Select a User"), tr("Choose a user from the list before resetting passwords."), QMessageBox::Warning);
        return;
    }

    const User &user = cachedUsers.at(row);
    bool accepted = false;
    const QString newPassword = QInputDialog::getText(
        this,
        tr("Reset Password"),
        tr("Enter a new password for %1:").arg(user.username),
        QLineEdit::Password,
        QString(),
        &accepted
    );

    if (!accepted) {
        return;
    }

    const QString trimmedPassword = newPassword.trimmed();
    if (trimmedPassword.isEmpty()) {
        showMessage(tr("Missing Password"), tr("Enter a new password before saving."), QMessageBox::Warning);
        return;
    }

    if (userDB.updatePassword(user.username, trimmedPassword)) {
        showMessage(tr("Password Reset"), tr("%1's password has been updated.").arg(user.username), QMessageBox::Information);
    } else {
        showMessage(tr("Reset Failed"), tr("Unable to reset this password. Please try again."), QMessageBox::Critical);
    }
}

void AdminTab::loadLoginBranding()
{
    ShopSettings settings = shopSettingsDB.loadSettings();

    if (headlineEdit) {
        headlineEdit->setText(settings.loginHeadline);
    }
    if (taglineEdit) {
        taglineEdit->setText(settings.loginTagline);
    }
    if (backgroundEdit) {
        backgroundEdit->setText(settings.loginBackgroundPath);
    }

    const QString normalizedAccent = normalizedColorString(settings.accentColor);
    if (accentPresetCombo) {
        int presetIndex = -1;
        for (int i = 0; i < accentPresetCombo->count(); ++i) {
            const QString data = accentPresetCombo->itemData(i).toString();
            if (!data.isEmpty() && !normalizedAccent.isEmpty() && data.compare(normalizedAccent, Qt::CaseInsensitive) == 0) {
                presetIndex = i;
                break;
            }
        }

        const int customIndex = accentPresetCombo->findData(QStringLiteral("custom"));
        if (presetIndex >= 0) {
            customAccentColor.clear();
            accentPresetCombo->setCurrentIndex(presetIndex);
        } else if (customIndex >= 0 && !normalizedAccent.isEmpty()) {
            customAccentColor = normalizedAccent;
            accentPresetCombo->setCurrentIndex(customIndex);
        } else if (accentPresetCombo->count() > 0) {
            accentPresetCombo->setCurrentIndex(0);
        }
    }

    if (headlineFontCombo && !settings.loginHeadlineFontFamily.isEmpty()) {
        headlineFontCombo->setCurrentFont(QFont(settings.loginHeadlineFontFamily));
    }
    if (taglineFontCombo && !settings.loginTaglineFontFamily.isEmpty()) {
        taglineFontCombo->setCurrentFont(QFont(settings.loginTaglineFontFamily));
    }

    if (!settings.loginTextColor.isEmpty()) {
        const QColor loaded(settings.loginTextColor);
        if (loaded.isValid()) {
            loginTextColor = loaded;
        }
    }

    handleAccentPresetChanged(accentPresetCombo ? accentPresetCombo->currentIndex() : 0);
    updateTextColorButton();
    updateLoginPreview();
}

void AdminTab::saveLoginBranding()
{
    ShopSettings settings = shopSettingsDB.loadSettings();

    if (headlineEdit) {
        settings.loginHeadline = headlineEdit->text().trimmed();
    }
    if (taglineEdit) {
        settings.loginTagline = taglineEdit->text().trimmed();
    }
    if (backgroundEdit) {
        settings.loginBackgroundPath = backgroundEdit->text().trimmed();
    }

    QString accentValue = currentAccentColor();
    const QString normalizedAccent = normalizedColorString(accentValue);
    if (normalizedAccent.isEmpty()) {
        showMessage(tr("Invalid Accent"), tr("Choose a valid accent color before saving."), QMessageBox::Warning);
        return;
    }
    settings.accentColor = normalizedAccent;

    if (headlineFontCombo) {
        settings.loginHeadlineFontFamily = headlineFontCombo->currentFont().family();
    }
    if (taglineFontCombo) {
        settings.loginTaglineFontFamily = taglineFontCombo->currentFont().family();
    }
    settings.loginTextColor = loginTextColor.isValid() ? loginTextColor.name(QColor::HexArgb) : QString();

    if (!shopSettingsDB.saveSettings(settings)) {
        showMessage(tr("Save Failed"), tr("Branding changes could not be saved. Please try again."), QMessageBox::Critical);
        return;
    }

    showMessage(tr("Branding Updated"), tr("Login screen customization saved successfully."), QMessageBox::Information);
    loadLoginBranding();
}

void AdminTab::browseForBackground()
{
    const QString current = backgroundEdit ? backgroundEdit->text() : QString();
    const QString path = QFileDialog::getOpenFileName(this, tr("Select Background"), current, tr("Images (*.png *.jpg *.jpeg *.bmp)"));
    if (!path.isEmpty() && backgroundEdit) {
        backgroundEdit->setText(path);
    }
    updateLoginPreview();
}

void AdminTab::pickAccentColor()
{
    QColor initial = QColor(customAccentColor);
    if (!initial.isValid()) {
        initial = QColor(QStringLiteral("#3A6EA5"));
    }
    const QColor chosen = QColorDialog::getColor(initial, this, tr("Choose Accent Color"));
    if (!chosen.isValid()) {
        return;
    }

    customAccentColor = chosen.alpha() == 255 ? chosen.name() : chosen.name(QColor::HexArgb);

    if (accentPresetCombo) {
        const int customIndex = accentPresetCombo->findData(QStringLiteral("custom"));
        if (customIndex >= 0 && accentPresetCombo->currentIndex() != customIndex) {
            accentPresetCombo->setCurrentIndex(customIndex);
            return; // handleAccentPresetChanged will update preview
        }
    }

    if (accentPickerBtn) {
        accentPickerBtn->setText(customAccentColor.toUpper());
    }
    updateLoginPreview();
}

void AdminTab::handleAccentPresetChanged(int index)
{
    if (!accentPresetCombo || !accentPickerBtn) {
        return;
    }

    const QString data = accentPresetCombo->itemData(index).toString();
    const bool isCustom = data == QStringLiteral("custom");
    accentPickerBtn->setEnabled(isCustom);

    if (isCustom) {
        if (customAccentColor.isEmpty() && !accentPresets.isEmpty()) {
            customAccentColor = accentPresets.first().second;
        }
        accentPickerBtn->setText(customAccentColor.isEmpty() ? tr("Choose Color") : customAccentColor.toUpper());
    } else {
        accentPickerBtn->setText(tr("Preset Color"));
        if (!data.isEmpty()) {
            customAccentColor.clear();
        }
    }

    updateLoginPreview();
}

QString AdminTab::currentAccentColor() const
{
    if (accentPresetCombo) {
        const QString data = accentPresetCombo->currentData().toString();
        if (data == QStringLiteral("custom")) {
            if (!customAccentColor.isEmpty()) {
                return customAccentColor;
            }
        } else if (!data.isEmpty()) {
            return data;
        }
    }

    if (!customAccentColor.isEmpty()) {
        return customAccentColor;
    }

    if (!accentPresets.isEmpty()) {
        return accentPresets.first().second;
    }

    return QStringLiteral("#3A6EA5");
}

void AdminTab::openTextColorDialog()
{
    const QColor chosen = QColorDialog::getColor(loginTextColor, this, tr("Choose Text Color"));
    if (!chosen.isValid()) {
        return;
    }
    loginTextColor = chosen;
    updateTextColorButton();
    updateLoginPreview();
}

void AdminTab::updateTextColorButton()
{
    if (!textColorBtn) {
        return;
    }
    const QColor color = loginTextColor.isValid() ? loginTextColor : QColor(QStringLiteral("#FFFFFF"));
    const QString name = color.name(QColor::HexArgb).toUpper();
    textColorBtn->setText(name);
    textColorBtn->setToolTip(tr("Current login text color: %1").arg(name));
}

void AdminTab::updateLoginPreview()
{
    if (!previewFrame) {
        return;
    }

    QColor accentColor(currentAccentColor());
    if (!accentColor.isValid()) {
        accentColor = QColor(QStringLiteral("#3A6EA5"));
    }

    const QString accentName = accentColor.name(QColor::HexArgb);

    QString backgroundRule = QStringLiteral("background-image: none;");
    if (backgroundEdit) {
        const QString path = backgroundEdit->text().trimmed();
        QFileInfo info(path);
        if (!path.isEmpty() && info.exists() && info.isFile()) {
            const QString url = QUrl::fromLocalFile(info.absoluteFilePath()).toString();
            backgroundRule = QStringLiteral("background-image: url(\"%1\"); background-position: center; background-repeat: no-repeat; background-origin: content;").arg(url);
            if (previewStatusLabel) {
                previewStatusLabel->setText(QString());
            }
        } else if (previewStatusLabel) {
            previewStatusLabel->setText(tr("Background preview unavailable. Select an image to see it here."));
        }
    }

    previewFrame->setStyleSheet(QStringLiteral(
        "#loginPreviewFrame {"
        "    border: 2px solid %1;"
        "    border-radius: 16px;"
        "    background-color: rgba(15, 23, 42, 200);"
        "    %2"
        "}"
    ).arg(accentName, backgroundRule));

    const QColor headlineColor = loginTextColor.isValid() ? loginTextColor : QColor(QStringLiteral("#FFFFFF"));
    QColor taglineColor = headlineColor;
    if (taglineColor.isValid()) {
        const int alpha = taglineColor.alpha() > 0 ? taglineColor.alpha() : 255;
        taglineColor.setAlpha(std::max(80, alpha * 85 / 100));
    }

    if (previewHeadlineLabel) {
        const QString headlineText = headlineEdit && !headlineEdit->text().trimmed().isEmpty()
            ? headlineEdit->text().trimmed()
            : tr("Let's get inking");
        previewHeadlineLabel->setText(headlineText);
        previewHeadlineLabel->setFont(headlineFontCombo ? headlineFontCombo->currentFont() : previewHeadlineLabel->font());
        previewHeadlineLabel->setStyleSheet(QStringLiteral("color: %1;").arg(headlineColor.name(QColor::HexArgb)));
    }

    if (previewTaglineLabel) {
        const QString taglineText = taglineEdit && !taglineEdit->text().trimmed().isEmpty()
            ? taglineEdit->text().trimmed()
            : tr("We're glad you're here");
        previewTaglineLabel->setText(taglineText);
        previewTaglineLabel->setFont(taglineFontCombo ? taglineFontCombo->currentFont() : previewTaglineLabel->font());
        previewTaglineLabel->setStyleSheet(QStringLiteral("color: %1;").arg(taglineColor.name(QColor::HexArgb)));
    }

    if (previewStatusLabel) {
        previewStatusLabel->setStyleSheet(QStringLiteral("color: %1;").arg(taglineColor.name(QColor::HexArgb)));
    }

    updateTextColorButton();
}

void AdminTab::updateTheme()
{
    Theme t = ThemeManager::instance()->currentTheme();
    const QColor defaultText = palette().color(QPalette::Text);
    const QColor textColor = t.textColor.isValid() ? t.textColor : (t.text.isValid() ? t.text : defaultText);
    const QColor accent = t.primary.isValid() ? t.primary : (t.accent.isValid() ? t.accent : textColor);
    const QColor fieldBg = t.background.isValid() ? t.background.lighter(112) : palette().color(QPalette::Base);
    const QColor border = t.borderColor.isValid() ? t.borderColor : (t.border.isValid() ? t.border : accent.darker(125));
    const QColor altRow = fieldBg.lighter(105);
    const QColor selection = accent.isValid() ? accent : textColor;
    QColor selectionText = textColor;
    if (selection == textColor) {
        selectionText = palette().color(QPalette::Base);
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

    for (QLineEdit *edit : textFields) {
        if (edit) {
            edit->setStyleSheet(fieldStyle);
        }
    }

    const QString comboStyle = QStringLiteral(
        "QComboBox {"
        "    background-color: %1;"
        "    border: 1px solid %2;"
        "    border-radius: 10px;"
        "    padding: 6px 10px;"
        "    color: %3;"
        "}"
        "QComboBox::drop-down {"
        "    width: 24px;"
        "    border: none;"
        "}"
        "QComboBox QAbstractItemView {"
        "    background-color: %1;"
        "    color: %3;"
        "    selection-background-color: %4;"
        "    selection-color: %5;"
        "    border: 1px solid %2;"
        "    border-radius: 8px;"
        "}")
        .arg(fieldBg.name(QColor::HexArgb))
        .arg(border.name())
        .arg(textColor.name())
        .arg(selection.name())
        .arg(selectionText.name());

    for (QComboBox *combo : comboFields) {
        if (combo) {
            combo->setStyleSheet(comboStyle);
        }
    }

    const QString secondaryStyle = QStringLiteral(
        "QPushButton {"
        "    background-color: %1;"
        "    color: %2;"
        "    border-radius: 10px;"
        "    padding: 6px 14px;"
        "    border: 1px solid %3;"
        "}"
        "QPushButton:hover {"
        "    background-color: %4;"
        "}")
        .arg(fieldBg.name(QColor::HexArgb))
        .arg(textColor.name())
        .arg(border.name())
        .arg(altRow.name(QColor::HexArgb));

    for (QPushButton *btn : secondaryButtons) {
        if (btn) {
            btn->setStyleSheet(secondaryStyle);
        }
    }

    styleButton(addUserBtn);
    styleButton(updateRoleBtn);
    styleButton(resetPasswordBtn);
    styleButton(saveBrandingBtn);

    if (selectedUserLabel) {
        selectedUserLabel->setStyleSheet(QStringLiteral("color: %1; font-weight: 600;").arg(textColor.name()));
    }

    if (userTable) {
        userTable->setStyleSheet(QStringLiteral(
            "QTableWidget {"
            "    background: %1;"
            "    alternate-background-color: %2;"
            "    border: 1px solid %3;"
            "    border-radius: 12px;"
            "    color: %4;"
            "    gridline-color: %3;"
            "}"
            "QTableWidget::item:selected {"
            "    background: %5;"
            "    color: %6;"
            "}"
            "QHeaderView::section {"
            "    background: %7;"
            "    color: %4;"
            "    border: none;"
            "    padding: 6px 10px;"
            "    font-weight: 600;"
            "    border-top-left-radius: 12px;"
            "    border-top-right-radius: 12px;"
            "}"
            "QTableCornerButton::section {"
            "    background: %7;"
            "    border: none;"
            "}")
            .arg(fieldBg.name(QColor::HexArgb))
            .arg(altRow.name(QColor::HexArgb))
            .arg(border.name())
            .arg(textColor.name())
            .arg(selection.name())
            .arg(selectionText.name())
            .arg(fieldBg.darker(108).name(QColor::HexArgb)));
    }

    if (adminCard) {
        adminCard->updateTheme();
    }
    if (userSection) {
        userSection->refreshTheme();
    }
    if (shopSection) {
        shopSection->refreshTheme();
    }
    if (loginSection) {
        loginSection->refreshTheme();
    }

    updateLoginPreview();
}

void AdminTab::showMessage(const QString &title, const QString &message, QMessageBox::Icon icon) const
{
    QMessageBox msgBox(icon, title, message, QMessageBox::Ok, const_cast<AdminTab *>(this));
    msgBox.exec();
}

