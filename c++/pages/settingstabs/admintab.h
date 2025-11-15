#pragma once
#include "themes/themeablewidget.h"
#include "db/userdb.h"
#include "db/shopsettingsdb.h"
#include "domain/shopsettings.h"

#include <QVector>
#include <QMessageBox>
#include <QColor>
#include <QPair>

class QLineEdit;
class QPushButton;
class QLabel;
class SettingsCard;
class QTableWidget;
class QComboBox;
class QFontComboBox;
class QFrame;
class CollapsibleSection;
class ShopSettingsTab;

class AdminTab : public ThemeableWidget
{
    Q_OBJECT
public:
    explicit AdminTab(UserDB &udb, ShopSettingDB &sdb, QWidget *parent = nullptr);

private:
    void updateTheme() override;

    UserDB &userDB;
    ShopSettingDB &shopSettingsDB;
    SettingsCard *adminCard = nullptr;
    CollapsibleSection *userSection = nullptr;
    CollapsibleSection *shopSection = nullptr;
    CollapsibleSection *loginSection = nullptr;
    ShopSettingsTab *embeddedShopSettings = nullptr;

    // User management controls
    QPushButton *addUserBtn = nullptr;
    QTableWidget *userTable = nullptr;
    QLabel *selectedUserLabel = nullptr;
    QPushButton *updateRoleBtn = nullptr;
    QPushButton *resetPasswordBtn = nullptr;

    // Login branding controls
    QLineEdit *headlineEdit = nullptr;
    QLineEdit *taglineEdit = nullptr;
    QComboBox *accentPresetCombo = nullptr;
    QLineEdit *backgroundEdit = nullptr;
    QFontComboBox *headlineFontCombo = nullptr;
    QFontComboBox *taglineFontCombo = nullptr;
    QPushButton *accentPickerBtn = nullptr;
    QPushButton *backgroundBrowseBtn = nullptr;
    QPushButton *textColorBtn = nullptr;
    QPushButton *saveBrandingBtn = nullptr;
    QFrame *previewFrame = nullptr;
    QLabel *previewHeadlineLabel = nullptr;
    QLabel *previewTaglineLabel = nullptr;
    QLabel *previewStatusLabel = nullptr;

    QVector<QPair<QString, QString>> accentPresets;
    QColor loginTextColor = QColor("#FFFFFF");
    QString customAccentColor;

    QVector<QLineEdit*> textFields;
    QVector<QComboBox*> comboFields;
    QVector<QPushButton*> secondaryButtons;
    QVector<QPair<QString, QString>> roleOptions;

    QList<User> cachedUsers;

    void initializeUserSection();
    void initializeShopSection();
    void initializeLoginSection();

    void reloadUsers();
    void onUserSelectionChanged(int currentRow);
    void handleAddUser();
    void handleRoleUpdate();
    void handlePasswordReset();

    void loadLoginBranding();
    void saveLoginBranding();
    void browseForBackground();
    void pickAccentColor();
    void handleAccentPresetChanged(int index);
    QString currentAccentColor() const;
    void openTextColorDialog();
    void updateTextColorButton();
    void updateLoginPreview();

    void showMessage(const QString &title, const QString &message, QMessageBox::Icon icon) const;
};
