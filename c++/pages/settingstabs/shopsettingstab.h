#pragma once
#include "themes/themeablewidget.h"
#include "db/shopsettingsdb.h"
#include <QLocale>

#include <QVector>

class QLineEdit;
class QPushButton;
class QLabel;
class SettingsCard;
struct ShopSettings;

class ShopSettingsTab : public ThemeableWidget
{
    Q_OBJECT
public:
    explicit ShopSettingsTab(ShopSettingDB &sdb, QWidget *parent = nullptr);

private:
    void updateTheme() override;
    void applyShopNameToAllWindows(const QString &shopName);
    void populateFromSettings(const ShopSettings &settings);
    bool tryParseDouble(QLineEdit *lineEdit, const QString &fieldLabel, double &value);
    void reloadSettings();
    void handleFieldChange(const QString &);
    void updateButtonStates();
    bool hasChanges() const;
    bool ratesAreValid() const;
    QWidget *createSectionHeader(const QString &iconPath, const QString &title);
    QLabel *createCaptionLabel(const QString &text);

    ShopSettingDB &shopSettings;
    SettingsCard *mainCard = nullptr;
    QLineEdit *shopNameEdit;
    QLineEdit *tattooRateEdit;
    QLineEdit *pierceSingleEdit;
    QLineEdit *pierceMultiEdit;
    QLineEdit *sidebarArtworkEdit;
    QPushButton *saveBtn;
    QPushButton *sidebarArtworkBrowseBtn;
    QVector<QLabel*> headingLabels;
    QVector<QLabel*> captionLabels;
    QVector<QPushButton*> secondaryButtons;
    QLabel *sidebarArtworkPreview = nullptr;
    ShopSettings lastLoadedSettings;
    bool hasPersistedSettings = false;
    bool isPopulatingFields = false;
    QLocale numericLocale = QLocale::c();

    void updateSidebarArtworkPreview(const QString &path);
};
